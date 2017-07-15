/*
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
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MARC.EHRS.Visualization.Client.Silverlight.UI.Config
{
    /// <summary>
    /// Configuration for the visualizer
    /// </summary>
    [XmlRoot("configuration")]
    public class Configuration
    {

        /// <summary>
        /// Diagrams
        /// </summary>
        [XmlElement("diagram")]
        public List<Diagram> Diagrams { get; set; }

        /// <summary>
        /// Servers
        /// </summary>
        [XmlElement("server")]
        public List<Server> Servers { get; set; }

        /// <summary>
        /// Sponsors
        /// </summary>
        [XmlElement("sponsor")]
        public List<String> Sponsors { get; set; }

        /// <summary>
        /// About
        /// </summary>
        [XmlAnyElement()]
        public XElement[] About { get; set; }

        /// <summary>
        /// Fired when the load is complete
        /// </summary>
        public static event EventHandler<ConfigurationLoadedEventArgs> LoadComplete;

        /// <summary>
        /// Load the configuration asynchronously
        /// </summary>
        public static void LoadAsync(string configFile)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
            wc.OpenReadAsync(new Uri(configFile));
        }

        /// <summary>
        /// Read completed
        /// </summary>
        private static void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            // Prepare an XmlSerializer
            XmlSerializer xsz = new XmlSerializer(typeof(Configuration));
            try
            {
                var config = xsz.Deserialize(e.Result) as Configuration;
                if(LoadComplete != null)
                    LoadComplete(null, new ConfigurationLoadedEventArgs() { Configuration = config });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
        }
    }
}
