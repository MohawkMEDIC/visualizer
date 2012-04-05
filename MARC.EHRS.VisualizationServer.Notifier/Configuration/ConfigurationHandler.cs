using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MARC.EHRS.VisualizationServer.Notifier.Configuration
{
    /// <summary>
    /// Handles the configuration for the notification service
    /// </summary>
    public class ConfigurationHandler : IConfigurationSectionHandler
    {

        /// <summary>
        /// Specifies the address to bind to
        /// </summary>
        public IPAddress BindAddress { get; set; }

        /// <summary>
        /// Specifies the port to bind to
        /// </summary>
        public int BindPort { get; set; }

        /// <summary>
        /// Enables the CAP Server
        /// </summary>
        public bool EnableCAPServer { get; set; }

        /// <summary>
        /// The policy file for the CAP server
        /// </summary>
        public string CapServerPolicyFile { get; set; }

        #region IConfigurationSectionHandler Members

        /// <summary>
        /// Create the configuration section
        /// </summary>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {

            if (section.Attributes["address"] != null)
                BindAddress = IPAddress.Parse(section.Attributes["address"].Value);
            else
            {
                Trace.TraceWarning("Couldn't find configuration for address, binding to *");
                BindAddress = IPAddress.Any;
            }

            if (section.Attributes["port"] != null)
                BindPort = Int32.Parse(section.Attributes["port"].Value);
            else
            {
                Trace.TraceWarning("Couldn't find configuration for bind port, default to 4530");
                BindPort = 4530;
            }

            if(section.Attributes["enableCAPServer"] != null)
                EnableCAPServer = Boolean.Parse(section.Attributes["enableCAPServer"].Value);

            if (section.Attributes["capServerPolicyFile"] != null)
            {
                CapServerPolicyFile = section.Attributes["capServerPolicyFile"].Value;
                if (!File.Exists(CapServerPolicyFile))
                    CapServerPolicyFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), CapServerPolicyFile);
            }

            // Sanity check
            if (BindPort < 4502 || BindPort > 4532)
                Trace.TraceWarning("Binding port '{0}' may prohibit Silverlight applications from connecting");

            return this;
        }

        #endregion
    }
}
