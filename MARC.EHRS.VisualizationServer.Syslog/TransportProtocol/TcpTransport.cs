﻿/**
 * Copyright 2012-2013 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 13-8-2012
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using MARC.EHRS.VisualizationServer.Syslog.Configuration;
using MARC.EHRS.VisualizationServer.Syslog.Exceptions;
using System.Text.RegularExpressions;

namespace MARC.EHRS.VisualizationServer.Syslog.TransportProtocol
{
    /// <summary>
    /// Transport protocol for TCP
    /// </summary>
    [Description("Syslog over TCP")]
    public class TcpTransport : ITransportProtocol
    {
        #region ITransportProtocol Members

        // The socket
        private TcpListener m_listener;
        
        // Will run while true
        private bool m_run = true;

        // Configuration
        private EndpointConfiguration m_configuration;

        /// <summary>
        /// Gets the name of the protocol
        /// </summary>
        public virtual string ProtocolName
        {
            get { return "tcp"; }
        }

        /// <summary>
        /// Start the handler
        /// </summary>
        public virtual void Start(EndpointConfiguration config)
        {

            // Get the IP address
            IPEndPoint endpoint = null;
            if (config.Address.HostNameType == UriHostNameType.Dns)
                endpoint = new IPEndPoint(Dns.Resolve(config.Address.Host).AddressList[0], config.Address.Port);
            else
                endpoint = new IPEndPoint(IPAddress.Parse(config.Address.Host), config.Address.Port);

            this.m_configuration = config;
            this.m_listener = new TcpListener(endpoint);
            this.m_listener.Start();
            Trace.TraceInformation("TCP Transport bound to {0}", endpoint);

            while (m_run) // run the service
            {
                // Client
                TcpClient client = this.m_listener.AcceptTcpClient();
                Trace.TraceInformation("Established syslog connection with {0}", client.Client.RemoteEndPoint);
                Thread clientThread = new Thread(OnReceiveMessage);
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }

        /// <summary>
        /// Receive and process message
        /// </summary>
        protected virtual void OnReceiveMessage(object client)
        {
            TcpClient tcpClient = client as TcpClient;
            NetworkStream stream = tcpClient.GetStream();
            try
            {
                stream.ReadTimeout = (int)this.m_configuration.ReadTimeout.TotalMilliseconds;
                this.ProcessSession(tcpClient, stream);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                stream.Close();
                tcpClient.Close();
            }
        }

        /// <summary>
        /// Process a TCP session
        /// </summary>
        protected void ProcessSession(TcpClient tcpClient, Stream stream)
        {
            // Now read to a string
            StringBuilder messageData = new StringBuilder();
            int nSessionMessages = 0;
            Guid sessionId = Guid.NewGuid();

            try
            {


                byte[] buffer = new byte[512];

                Regex messageLengthMatch = new Regex(@"^(\d*)\s(.*)$");
                DateTime startConnection = DateTime.Now;

                while (tcpClient.Connected)
                {
                    try
                    {
                        if (DateTime.Now.Subtract(startConnection) > this.m_configuration.Timeout)
                            throw new TimeoutException();

                        int br = stream.Read(buffer, 0, 512);
                        nSessionMessages++;
                        messageData.Append(Encoding.UTF8.GetString(buffer, 0, br));
                    }
                    catch (TimeoutException e)
                    {
                        Trace.TraceError("{0} : Timeout occurred! Killing connection", tcpClient.Client.RemoteEndPoint);
                        throw;
                    }
                    catch (IOException)
                    {
                        Trace.TraceWarning("{0} : No data received on connection waiting {1} more seconds", tcpClient.Client.RemoteEndPoint, this.m_configuration.Timeout.Subtract(DateTime.Now.Subtract(startConnection)).TotalSeconds);
                    }

                    // Check ... Does the message start with a size ?
                    string currentMessageData = messageData.ToString();
                    var match = messageLengthMatch.Match(currentMessageData);
                    if (match.Success)
                        while (true)
                        {
                            string msgLength = match.Groups[1].Value;
                            int length = 0;
                            if (Int32.TryParse(msgLength, out length) && match.Groups[2].Value.Length >= length) // Complete message!
                            {

                                var strMessage = match.Groups[2].Value.Substring(0, length);
                                ProcessSyslogMessage(strMessage, tcpClient, sessionId);
                                if (match.Groups[2].Value.Length > length) // more message
                                {
                                    messageData = new StringBuilder(match.Groups[2].Value.Substring(length));
                                    currentMessageData = messageData.ToString();
                                    match = messageLengthMatch.Match(currentMessageData);
                                }
                                else
                                {
                                    messageData = new StringBuilder();
                                    break;
                                }
                            }
                            else
                                break;
                        }
                    else if (currentMessageData.Contains("\n")) // Separated by newline!
                    {
                        var strMessage = currentMessageData;

                        if (!strMessage.EndsWith("\n")) // Not a full message
                        {
                            strMessage = strMessage.Substring(0, strMessage.LastIndexOf("\n") + 1);
                            messageData = new StringBuilder(strMessage.Substring(strMessage.LastIndexOf("\n") + 1));
                        }
                        else
                        {
                            messageData = new StringBuilder();
                        }

                        ProcessSyslogMessage(strMessage, tcpClient, sessionId);
                    }

                    // else
                }


            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                if (messageData.Length > 0)
                    ProcessSyslogMessage(messageData.ToString(), tcpClient, sessionId);

                if (nSessionMessages == 0)
                    Trace.TraceInformation("{0} : Client did not send data in specified amount of time!", tcpClient.Client.RemoteEndPoint);
                else
                    Trace.TraceInformation("Finished syslog connection with {0}", tcpClient.Client.RemoteEndPoint);
            }

        }

        /// <summary>
        /// Process a syslog message
        /// </summary>
        private void ProcessSyslogMessage(string strMessage, TcpClient client, Guid sessionId)
        {
            var localEp = client.Client.LocalEndPoint as IPEndPoint;
            var remoteEp = client.Client.RemoteEndPoint as IPEndPoint;
            Uri localEndpoint = new Uri(String.Format("tcp://{0}:{1}", localEp.Address, localEp.Port));
            Uri remoteEndpoint = new Uri(String.Format("tcp://{0}:{1}", remoteEp.Address, remoteEp.Port));

            // Get rid of length
            Regex messageLengthMatch = new Regex(@"^(\d*)\s(.*)$");
            var match = messageLengthMatch.Match(strMessage);
            if (match.Success)
                strMessage = match.Groups[2].Value;

            if (strMessage.Length == 0) return; // no message 

            try
            {
                var message = SyslogMessage.Parse(strMessage, sessionId);
                var messageArgs = new SyslogMessageReceivedEventArgs(message, remoteEndpoint, localEndpoint, DateTime.Now);

                this.FireMessageReceived(this, messageArgs);

                // Forward
                TransportUtil.Current.Forward(this.m_configuration.Forward, Encoding.UTF8.GetBytes(strMessage));
            }
            catch (SyslogMessageException e)
            {
                this.FireInvalidMessageReceived(this, new SyslogMessageReceivedEventArgs(e.FaultingMessage, remoteEndpoint, localEndpoint, DateTime.Now));
                Trace.TraceError(e.Message);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        /// <summary>
        /// Message is received
        /// </summary>
        protected void FireMessageReceived(Object sender, SyslogMessageReceivedEventArgs messageArgs)
        {
            if (this.MessageReceived != null)
                this.MessageReceived.BeginInvoke(sender, messageArgs, null, null);

        }

        /// <summary>
        /// Invalid message is received
        /// </summary>
        protected void FireInvalidMessageReceived(Object sender, SyslogMessageReceivedEventArgs messageArgs)
        {
            if (this.InvalidMessageReceived != null)
                this.InvalidMessageReceived.BeginInvoke(sender, messageArgs, null, null);
        }

        /// <summary>
        /// Stop the thread
        /// </summary>
        public void Stop()
        {
            this.m_run = false;
            this.m_listener.Stop();
            Trace.TraceInformation("TCP Transport stopped");

        }

        /// <summary>
        /// Message has been received
        /// </summary>
        public event EventHandler<SyslogMessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// Invalid message has been received
        /// </summary>
        public event EventHandler<SyslogMessageReceivedEventArgs> InvalidMessageReceived;

        /// <summary>
        /// Forwrd the message on TCP
        /// </summary>
        public virtual void Forward(EndpointConfiguration config, byte[] rawMessage)
        {

            // Get the IP address
            IPEndPoint endpoint = null;
            if (config.Address.HostNameType == UriHostNameType.Dns)
                endpoint = new IPEndPoint(Dns.Resolve(config.Address.Host).AddressList[0], config.Address.Port);
            else
                endpoint = new IPEndPoint(IPAddress.Parse(config.Address.Host), config.Address.Port);

            TcpClient tcpClient = new TcpClient();
            try
            {
                tcpClient.Connect(endpoint);

                // Write to the stream
                NetworkStream stream = tcpClient.GetStream();
                stream.Write(rawMessage, 0, rawMessage.Length);
                stream.Flush();
                
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                tcpClient.Close();
            }
        }

        #endregion
    }
}
