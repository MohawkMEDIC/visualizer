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
        /// Create the configuration handler
        /// </summary>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {

            VisualizerConfigurationSection retVal = new VisualizerConfigurationSection();

            // Sections
            var endpoints = section.SelectNodes("./*[local-name() = 'listener']");
            if (endpoints.Count == 0)
                throw new ConfigurationErrorsException("Cannot find any application endpoints for Syslog message processing", section);
            foreach (XmlNode ep in endpoints)
                retVal.Endpoints.Add(this.ProcessEndpointConfiguration(ep));

            return retVal;
        }

        /// <summary>
        /// Process endpoint configuration
        /// </summary>
        private EndpointConfiguration ProcessEndpointConfiguration(XmlNode ep)
        {
            // Prepare the EP
            EndpointConfiguration config = new EndpointConfiguration();

            // name & address are mandatory
            if (ep.Attributes["name"] != null)
                config.Name = ep.Attributes["name"].Value;
            else
                throw new ConfigurationErrorsException("Must carry a name attribute", ep);

            if (ep.Attributes["address"] != null)
                config.Address = new Uri(ep.Attributes["address"].Value);
            else
                throw new ConfigurationErrorsException("Must carry an address attribute", ep);

            // Handler
            if (ep.Attributes["action"] != null)
            {
                Type handlerType = Type.GetType(ep.Attributes["action"].Value);
                if (handlerType == null)
                    throw new ConfigurationErrorsException("Cannot find the specified type", ep.Attributes["action"]);
                config.Action = handlerType;
            }

            if (ep.Attributes["maxSize"] != null)
                config.MaxSize = Int32.Parse(ep.Attributes["maxSize"].Value);
            else
                config.MaxSize = 1024;
            // Now process the attributes if any
            foreach (XmlNode att in ep.SelectNodes("./*[local-name() = 'attribute']"))
            {
                if (att.Attributes["name"] == null || att.Attributes["value"] == null)
                    throw new ConfigurationErrorsException("Missing name and/or value on attribute", ep);
                KeyValuePair<String, String> attValue = new KeyValuePair<string, string>(att.Attributes["name"].Value, att.Attributes["value"].Value);
            }

            // Now endpoints 
            foreach (XmlNode fwd in ep.SelectNodes("./*[local-name() = 'forward']"))
                config.Forward.Add(this.ProcessEndpointConfiguration(fwd));

            return config;
        }

        #endregion
    }
}
