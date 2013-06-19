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

namespace MARC.EHRS.VisualizationClient.Silverlight.Config
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
