using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Xml;

namespace MARC.EHRS.VisualizationServer.Syslog.Configuration
{
    /// <summary>
    /// Handler for the syslog configuration
    /// </summary>
    public class ConfigurationHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        /// <summary>
        /// Binding configuration
        /// </summary>
        public BindingConfiguration[] Bindings { get; set; }

        /// <summary>
        /// Binding configuration for sockets
        /// </summary>
        public struct BindingConfiguration
        {

            /// <summary>
            /// Gets or sets the port that syslog listens on
            /// </summary>
            public int BindPort { get; set; }

            /// <summary>
            /// Bind to address
            /// </summary>
            public IPAddress BindAddress { get; set; }

            /// <summary>
            /// Port to forward
            /// </summary>
            public int ForwardPort { get; set; }

            /// <summary>
            /// Forward to address
            /// </summary>
            public IPAddress ForwardAddress { get; set; }

        }

        /// <summary>
        /// Gets or sets the maximum UDP size
        /// </summary>
        public int MaxUdpSize { get; private set; }


        /// <summary>
        /// Create the configuration handler
        /// </summary>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {

            var bindings = section.SelectNodes("//*[local-name() = 'binding']");
            List<BindingConfiguration> configBind = new List<BindingConfiguration>(bindings.Count);
            Trace.TraceInformation("Binding to '{0}' interfaces...", bindings.Count);
            foreach (XmlElement binding in bindings)
            {
                BindingConfiguration bc = new BindingConfiguration();

                // Read the configuration section
                if (binding.Attributes["port"] != null)
                    bc.BindPort = Int32.Parse(binding.Attributes["port"].Value);
                else
                {
                    Trace.TraceWarning("No port specified, assuming port 514");
                    bc.BindPort = 514;
                }

                if (binding.Attributes["address"] != null)
                    bc.BindAddress = IPAddress.Parse(binding.Attributes["address"].Value);
                else
                {
                    Trace.TraceWarning("No address specified, binding to *");
                    bc.BindAddress = IPAddress.Any;
                }

                // Read the configuration section
                if (binding.Attributes["forwardPort"] != null)
                    bc.ForwardPort = Int32.Parse(binding.Attributes["forwardPort"].Value);
                else
                    bc.ForwardPort = 514;

                if (binding.Attributes["forwardAddress"] != null)
                    bc.ForwardAddress = IPAddress.Parse(binding.Attributes["forwardAddress"].Value);


                configBind.Add(bc);
            }



            if (section.Attributes["maxMessageSize"] != null)
                this.MaxUdpSize = Int32.Parse(section.Attributes["maxMessageSize"].Value);
            else
            {
                Trace.TraceWarning("No max message size specified, assuming {0}", short.MaxValue);
                this.MaxUdpSize = short.MaxValue;
            }

            this.Bindings = configBind.ToArray();

            return this;
        }

        #endregion
    }
}
