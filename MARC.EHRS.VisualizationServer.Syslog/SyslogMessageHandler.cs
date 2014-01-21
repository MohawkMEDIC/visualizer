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
using AtnaApi.Model;
using System.Globalization;
using MARC.EHRS.Visualization.Core.Services;
using MARC.EHRS.VisualizationServer.Syslog.Configuration;
using System.Threading;
using System.Text.RegularExpressions;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;

namespace MARC.EHRS.VisualizationServer.Syslog
{
    public class SyslogMessageHandler : IMessageHandlerService
    {

        // Configuration
        private VisualizerConfigurationSection m_configuration;

        public SyslogMessageHandler()
        {
            this.m_configuration = ConfigurationManager.GetSection("marc.ehrs.visualizationserver.syslog") as VisualizerConfigurationSection;
        }

        #region IMessageHandlerService Members

        /// <summary>
        /// Start the message handler
        /// </summary>
        public bool Start()
        {
            ApplicationContext.CurrentContext = this.Context;

            foreach (var ep in this.m_configuration.Endpoints)
            {
                var sh = new ListenerThread(ep);
                Thread thdSh = new Thread(sh.Run);
                thdSh.IsBackground = true;
                Trace.TraceInformation("Starting Syslog Listener '{0}'...", ep.Name);
                thdSh.Start();
            }
            return true;
        }



        /// <summary>
        /// Stop the message handler
        /// </summary>
        public bool Stop()
        {
            return true; // background threads just get ended
        }

        #endregion

        #region IUsesHostContext Members

        /// <summary>
        /// Gets or sets the context
        /// </summary>
        public IServiceProvider Context
        {
            get;
            set;
        }

        #endregion
    }
}
