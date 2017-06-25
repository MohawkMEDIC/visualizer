using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using MARC.EHRS.VisualizationServer.Syslog.Configuration;
using System.Threading;
using MARC.HI.EHRS.SVC.Core.Services;
using System.IO;
using System.Diagnostics;

namespace MARC.EHRS.VisualizationServer.Syslog
{
    /// <summary>
    /// Represents an endpoint listener endpoint thread
    /// </summary>
    public class ListenerThread 
    {

        // The transport protocol
        private ITransportProtocol m_protocol;

        // The message handler
        private List<ISyslogAction> m_action = new List<ISyslogAction>();
        
        // Endpoint configuration
        private EndpointConfiguration m_configuration;

        /// <summary>
        /// Listener thread
        /// </summary>
        public ListenerThread(EndpointConfiguration config)
        {
            this.m_configuration = config;
            this.m_protocol = TransportUtil.Current.CreateTransport(config.Address.Scheme);
            this.m_protocol.MessageReceived += new EventHandler<SyslogMessageReceivedEventArgs>(m_protocol_MessageReceived);
            this.m_protocol.InvalidMessageReceived += new EventHandler<SyslogMessageReceivedEventArgs>(m_protocol_InvalidMessageReceived);
            foreach (var act in this.m_configuration.Action)
            {
                var handler = Activator.CreateInstance(act) as ISyslogAction;
                if (this.m_action == null)
                    throw new InvalidOperationException("Action does not implement ISyslogAction interface");
                handler.Context = ApplicationContext.CurrentContext;
                this.m_action.Add(handler);
            }
        }

        /// <summary>
        /// Invalid message is received
        /// </summary>
        void m_protocol_InvalidMessageReceived(object sender, SyslogMessageReceivedEventArgs e)
        {

            // Store
            PersistMessageEvent(e);

            // Perform actions
            foreach (var act in this.m_action)
                try {
                    act.HandleInvalidMessage(sender, e);
                }
                catch(Exception ex)
                {
                    Trace.TraceError("Error executing action {0}: {1}", act, ex.ToString());
                }
        }

        /// <summary>
        /// Message has been received
        /// </summary>
        void m_protocol_MessageReceived(object sender, SyslogMessageReceivedEventArgs e)
        {

            // Perform actions
            foreach(var act in this.m_action)
                try
                {
                    act.HandleMessageReceived(sender, e);
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Error executing action {0}: {1}", act, ex.ToString());
                }

        }

        /// <summary>
        /// Persist message events
        /// </summary>
        private void PersistMessageEvent(SyslogMessageReceivedEventArgs e)
        {
            IMessagePersistenceService msgPersistence = ApplicationContext.CurrentContext.GetService(typeof(IMessagePersistenceService)) as IMessagePersistenceService;

            if (msgPersistence == null) return; // no persistence service so exit
            
            // Write the message to a stream
            using (MemoryStream ms = new MemoryStream())
            {
                byte[] data = Encoding.UTF8.GetBytes(e.Message.Original);
                ms.Write(data, 0, data.Length);
                ms.Seek(0, SeekOrigin.Begin);

                // Now persist
                msgPersistence.PersistMessage(e.Message.CorrelationId.ToString(), ms);
            }
        }


        /// <summary>
        /// Run the service
        /// </summary>
        public void Run()
        {
            try
            {
                this.m_protocol.Start(this.m_configuration);
            }
            catch (ThreadAbortException)
            {
                this.m_protocol.Stop();
            }
        }

    }
}
