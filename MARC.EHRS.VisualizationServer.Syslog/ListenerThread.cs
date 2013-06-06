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
        private ISyslogMessageHandler m_handler;
        
        // Endpoint configuration
        private EndpointConfiguration m_configuration;

        /// <summary>
        /// Listener thread
        /// </summary>
        public ListenerThread(EndpointConfiguration config)
        {
            this.m_protocol = TransportUtil.Current.CreateTransport(config.Address.Scheme);
            this.m_handler = Activator.CreateInstance(config.Handler) as ISyslogMessageHandler;
            this.m_handler.Context = ApplicationContext.CurrentContext;
            this.m_configuration = config;
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
