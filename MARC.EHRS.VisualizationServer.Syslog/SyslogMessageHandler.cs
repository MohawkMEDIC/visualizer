using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Net.Sockets;
using System.Net;
using System.Configuration;
using System.Diagnostics;
using MARC.EHRS.Visualization.Core;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using MARC.HI.EHRS.SVC.Auditing.Atna.Format;
using System.Globalization;
using MARC.EHRS.Visualization.Core.Services;
using MARC.EHRS.VisualizationServer.Syslog.Configuration;
using System.Threading;
using System.Text.RegularExpressions;

namespace MARC.EHRS.VisualizationServer.Syslog
{
    public class SyslogMessageHandler : IMessageHandlerService
    {

        /// <summary>
        /// Sequence
        /// </summary>
        private long m_sequence = 0;

        /// <summary>
        /// The configuration of this listener
        /// </summary>
        private ConfigurationHandler m_configuration;

        // Listener thread
        List<Thread> m_listenerThreads = new List<Thread>();

        /// <summary>
        /// Default constructor
        /// </summary>
        public SyslogMessageHandler()
        {
            this.m_configuration = ConfigurationManager.GetSection("marc.ehrs.visualizationserver.syslog") as ConfigurationHandler;
        }

        #region IMessageHandlerService Members

        /// <summary>
        /// Start the message handler
        /// </summary>
        public bool Start()
        {
            // Start up the notification service
            INotificationService svc = this.Context.GetService(typeof(INotificationService)) as INotificationService;
            if (svc != null)
                svc.Start();
            else
                Trace.TraceWarning("Cannot find a notification service");

            
            // Start the listener thread.
            foreach (var config in this.m_configuration.Bindings)
            {
                var thd = new Thread(new ParameterizedThreadStart(UdpListen));
                thd.IsBackground = true;
                thd.Start(config);
                m_listenerThreads.Add(thd);
            }

            return true;
        }

        /// <summary>
        /// Stop the message handler
        /// </summary>
        public bool Stop()
        {
            foreach(var thd in this.m_listenerThreads)
                if (thd.IsAlive)
                    thd.Abort();

            // Start up the notification service
            INotificationService svc = this.Context.GetService(typeof(INotificationService)) as INotificationService;
            if (svc != null)
                svc.Stop();
            else
                Trace.TraceWarning("Cannot find a notification service");

            return true;
        }

        #endregion

        #region IUsesHostContext Members

        /// <summary>
        /// Gets or sets the context that this handler runs within
        /// </summary>
        public MARC.HI.EHRS.SVC.Core.HostContext Context
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Bind the socket to the localhost specified port.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private Socket CreateSocket(ConfigurationHandler.BindingConfiguration config)
        {

            try
            {

                var udpSocket = new Socket(AddressFamily.InterNetwork,
                    SocketType.Dgram, ProtocolType.Udp);
                udpSocket.DontFragment = true;
                var localEP = new IPEndPoint(config.BindAddress, config.BindPort);

                udpSocket.Bind(localEP);
                Trace.TraceInformation("Started listening on {0}", localEP.ToString());
                return udpSocket;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        /// <summary>
        /// Listen on the udp port
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "bytesReceived"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void UdpListen(object parm)
        {
            var config = (ConfigurationHandler.BindingConfiguration)parm;
            var udpSocket = CreateSocket(config);

            // If binding was successful, start listening.
            if (udpSocket.IsBound)
            {
                int bytesReceived = 0;

                try
                {
                    while (true)
                    {
                        try
                        {
                            Byte[] udpMessage = new Byte[m_configuration.MaxUdpSize];

                            //bytesReceived = udpSocket.Receive(udpMessage);
                            EndPoint remote_ep = new IPEndPoint(IPAddress.Any, 0);
                            bytesReceived = udpSocket.ReceiveFrom(udpMessage, ref remote_ep);

                            IPEndPoint ipep = (IPEndPoint)remote_ep;
                            IPAddress ipadd = ipep.Address;

                            // Forward the message
                            if (bytesReceived > 0 && config.ForwardAddress != null)
                                this.UdpForward(udpMessage, bytesReceived, config.ForwardAddress, config.ForwardPort);

                            // Parse
                            String udpMessageStr = System.Text.Encoding.UTF8.GetString(udpMessage).TrimEnd('\0');

                            
                            // 
                            //NotifyMessage(udpMessageStr, config.BindPort);
                            
                            NotifyMessage(udpMessageStr, ipadd, config.BindPort);
                            

                            udpMessage = null;
                        }
                        catch (ThreadAbortException)
                        {
                            throw;
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError("Error receiving udp message: {0}", e);
                        }
                    }
                }
                catch (ThreadAbortException e)
                {
                    Trace.TraceError(e.Message);
                    udpSocket.Close();
                }
                    

            }
        }

        /// <summary>
        /// Forward UDP message
        /// </summary>
        /// <param name="buffer">The buffer for the message to forward</param>
        /// <param name="byteSize">The size of the message</param>
        private void UdpForward(byte[] buffer, int byteSize, IPAddress forwardAddress, int forwardPort)
        {
            if(forwardAddress == null)
                return;

            // Endpoint
            IPEndPoint endpoint = new IPEndPoint(forwardAddress, forwardPort);

            // Client
            UdpClient udpClient = new UdpClient();
            try
            {

                udpClient.Connect(endpoint);
                
                // Send the message
                // Create the dgram
                byte[] dgram = new byte[byteSize];
                Array.Copy(buffer, dgram, byteSize);

                Trace.TraceInformation("Forwarding to '{1}' content '{0}'...", System.Text.Encoding.UTF8.GetString(dgram), endpoint.ToString());
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

        /// <summary>
        /// Routes messages to notifier.
        /// </summary>
        /// <param name="message">Message to route.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void NotifyMessage(string message, IPAddress ip, int source)
        {
            VisualizationEvent evt = ConvertToVisualization(message, ip, source);
            evt.Sequence = m_sequence++;

            if (evt != null)
            {
                INotificationService notif = Context.GetService(typeof(INotificationService)) as INotificationService;
                if (notif == null)
                    Trace.TraceError("Can't broadcast the message as no notification service exists");
                else
                    notif.Notify(evt);
            }
            else
            {
                Trace.TraceError("Could not create visualization event");
            }
        }

        /// <summary>
        /// Converts an audit message to a log message.
        /// </summary>
        /// <param name="message">Audit Message in serialized string format.</param>
        /// <returns>A string serialized format of a log message.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private VisualizationEvent ConvertToVisualization(string message, IPAddress sourceIP, int sourcePort)
        {
            VisualizationEvent logMessage = null;

            // Clean the message
            message = Regex.Replace(message, "[&]([^a][^m][^p])", "&amp;$1");

            int auditMsgStart = message.IndexOf("<AuditMessage", StringComparison.Ordinal);
            if (auditMsgStart >= 0)
            {
                Trace.TraceInformation("Received audit message to forward from IP '{0}' content: '{1}'", sourceIP.ToString(), message);
                try
                {
                    // Deserialize audit message.
                    string auditMessageStr = message.Substring(auditMsgStart);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AuditMessage));
                    StringReader strReader = new StringReader(auditMessageStr);
                    XmlTextReader xmlTextReader = new XmlTextReader(strReader);
                    AuditMessage auditMessage = (AuditMessage)xmlSerializer.Deserialize(xmlTextReader);

                    // Create a LogMessage equivalent.
                    if (auditMessage != null && auditMessage.SourceIdentification.Count > 0)
                    {
                        // Use the Enterprise site ID as the audit source ID.
                        // If not available, use the usual audit source ID.
                        string sourceID = "";

                        sourceID = auditMessage.SourceIdentification[0].AuditEnterpriseSiteID;
                        Trace.TraceInformation("Using enterprise site id: {0}" , sourceID);
                        if (String.IsNullOrEmpty(sourceID))
                        {
                            sourceID = auditMessage.SourceIdentification[0].AuditSourceID;
                            if (sourceID.Contains("@"))
                                sourceID = sourceID.Substring(0, sourceID.IndexOf("@"));
                            Trace.TraceInformation("Using audit source id: {0}", sourceID);
                        }

                        // Create the log message
                        logMessage = new VisualizationEvent()
                        {
                            MachineOID = sourceID
                            
                        };
                        if (sourceIP != null)
                        {
                            Trace.TraceInformation("Received message from : {0}", sourceIP.ToString());
                            logMessage.IPAddress = sourceIP.ToString();
                        }
                        if (auditMessage.EventIdentification != null)
                            logMessage.TimeStamp = auditMessage.EventIdentification.EventDateTime;
                        if (auditMessage.EventIdentification.EventType != null &&
                            auditMessage.EventIdentification.EventType.Count > 0)
                            logMessage.EventType = auditMessage.EventIdentification.EventType[0].Code;
                        if (auditMessage.EventIdentification != null && auditMessage.EventIdentification.EventId != null)
                            logMessage.EventID = auditMessage.EventIdentification.EventId.Code.ToString();
                        // Try to resolve the OID
                        ISystemConfigurationService configService = Context.GetService(typeof(ISystemConfigurationService)) as ISystemConfigurationService;
                        if (configService != null)
                        {
                            var data = configService.OidRegistrar.FindData(sourceID);
                            if (data != null)
                                logMessage.Name = data.Description;
                        }

                    }

                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                    Trace.TraceError(message);
                }
            }

            return logMessage;
        }

    }
}
