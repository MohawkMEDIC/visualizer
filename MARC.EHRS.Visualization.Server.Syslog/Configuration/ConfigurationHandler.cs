﻿/*
 * Copyright 2012-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2012-6-15
 */
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
            foreach(XmlNode xmlEl in ep.SelectNodes("./*[local-name() = 'actions']/*[local-name() = 'add']"))
            {
                Type handlerType = Type.GetType(xmlEl.InnerText);
                if (handlerType == null)
                    throw new ConfigurationErrorsException("Cannot find the specified type", xmlEl);
                config.Action.Add(handlerType);
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

            // Process timeout
            if (ep.Attributes["sessionTimeout"] != null)
                config.Timeout = TimeSpan.Parse(ep.Attributes["sessionTimeout"].Value);

            if (ep.Attributes["readTimeout"] != null)
                config.ReadTimeout = TimeSpan.Parse(ep.Attributes["readTimeout"].Value);
            return config;
        }

        #endregion
    }
}
