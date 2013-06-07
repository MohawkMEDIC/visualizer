using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using MARC.EHRS.VisualizationServer.Syslog.Configuration;
using System.Threading;

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
        private ISyslogAction m_action;
        
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
            this.m_action = Activator.CreateInstance(this.m_configuration.Action) as ISyslogAction;
            if (this.m_action == null)
                throw new InvalidOperationException("Action does not implement ISyslogAction interface");
            this.m_action.Context = ApplicationContext.CurrentContext;
        }

        /// <summary>
        /// Invalid message is received
        /// </summary>
        void m_protocol_InvalidMessageReceived(object sender, SyslogMessageReceivedEventArgs e)
        {
            this.m_action.HandleInvalidMessage(sender, e);
        }

        /// <summary>
        /// Message has been received
        /// </summary>
        void m_protocol_MessageReceived(object sender, SyslogMessageReceivedEventArgs e)
        {
            this.m_action.HandleMessageReceived(sender, e);
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
            catch (ThreadAbortException ta)
            {
                this.m_protocol.Stop();
            }
        }

    }
}
