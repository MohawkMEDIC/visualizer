/**
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
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.ComponentModel;
using MARC.EHRS.VisualizationServer.Syslog.Configuration;
using MARC.EHRS.VisualizationServer.Syslog.Exceptions;

namespace MARC.EHRS.VisualizationServer.Syslog.TransportProtocol
{
    /// <summary>
    /// HL7 llp transport
    /// </summary>
    [Description("Syslog over UDP")]
    public class UdpTransport : ITransportProtocol
    {

        // Socket
        private Socket m_udpSocket = null;

        // While true, run
        private bool m_run = true;

        // Endpoint configuration
        private EndpointConfiguration m_configuration;

        #region ITransportProtocol Members

        /// <summary>
        /// Get the protocol name
        /// </summary>
        public string ProtocolName
        {
            get { return "udp"; }
        }

        /// <summary>
        /// Start the listener
        /// </summary>
        /// <param name="bind"></param>
        public void Start(Configuration.EndpointConfiguration config)
        {
            this.m_udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.m_udpSocket.DontFragment = true;
            this.m_configuration = config;

            // Get the IP address
            IPEndPoint endpoint = null;
            if (config.Address.HostNameType == UriHostNameType.Dns)
                endpoint = new IPEndPoint(Dns.Resolve(config.Address.Host).AddressList[0], config.Address.Port);
            else
                endpoint = new IPEndPoint(IPAddress.Parse(config.Address.Host), config.Address.Port);

            // Bind the socket
            this.m_udpSocket.Bind(endpoint);
            Trace.TraceInformation("UDP transport bound to {0}", endpoint);

            // Run
            while (this.m_run)
            {
                // Client
                var client = this.m_udpSocket.Accept();
                Thread clientThread = new Thread(OnReceiveMessage);
                clientThread.IsBackground = true;
                clientThread.Start(client);
            }
        }

        /// <summary>
        /// Received a message
        /// </summary>
        protected void OnReceiveMessage(object client)
        {
            var udpSocket = client as Socket;
            EndPoint remote_ep = new IPEndPoint(IPAddress.Any, 0);

            try
            {
                Byte[] udpMessage = new Byte[this.m_configuration.MaxSize];

                //bytesReceived = udpSocket.Receive(udpMessage);
                int bytesReceived = udpSocket.ReceiveFrom(udpMessage, ref remote_ep);

                IPEndPoint ipep = (IPEndPoint)remote_ep;
                IPAddress ipadd = ipep.Address;

                // Parse
                String udpMessageStr = System.Text.Encoding.UTF8.GetString(udpMessage).TrimEnd('\0');
                var message = SyslogMessage.Parse(udpMessageStr);
                if (this.MessageReceived != null)
                    this.MessageReceived.BeginInvoke(this, new SyslogMessageReceivedEventArgs(message, new Uri(String.Format("udp://{0}", remote_ep)), this.m_configuration.Address, DateTime.Now), null, null);

                // Forward
                TransportUtil.Current.Forward(this.m_configuration.Forward, udpMessage);
            }
            catch (SyslogMessageException e)
            {
                if (this.InvalidMessageReceived != null)
                    this.InvalidMessageReceived.BeginInvoke(this, new SyslogMessageReceivedEventArgs(e.FaultingMessage, new Uri(String.Format("udp://{0}", remote_ep)), this.m_configuration.Address, DateTime.Now), null, null);
                Trace.TraceError(e.ToString());
            }
            finally
            {
                udpSocket.Dispose();
            }
        }

        /// <summary>
        /// Stop the process
        /// </summary>
        public void Stop()
        {
            this.m_run = false;
            this.m_udpSocket.Dispose();
        }

        /// <summary>
        /// A message was received
        /// </summary>
        public event EventHandler<SyslogMessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// An invalid message was received
        /// </summary>
        public event EventHandler<SyslogMessageReceivedEventArgs> InvalidMessageReceived;

        /// <summary>
        /// Forward on a UDP protocol
        /// </summary>
        public void Forward(EndpointConfiguration config, byte[] rawMessage)
        {
            // Get the IP address
            IPEndPoint endpoint = null;
            if (config.Address.HostNameType == UriHostNameType.Dns)
                endpoint = new IPEndPoint(Dns.Resolve(config.Address.Host).AddressList[0], config.Address.Port);
            else
                endpoint = new IPEndPoint(IPAddress.Parse(config.Address.Host), config.Address.Port);

            // Client
            UdpClient udpClient = new UdpClient();
            try
            {

                udpClient.Connect(endpoint);

                // Send the message
                // Create the dgram
                byte[] dgram = new byte[rawMessage.Length];
                Array.Copy(rawMessage, dgram, rawMessage.Length);
                udpClient.Send(dgram, (int)dgram.Length);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                udpClient.Close();
            }
        }

        #endregion
    }
}
