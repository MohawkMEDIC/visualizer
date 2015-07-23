using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.VisualizationServer.Syslog.Configuration
{
    /// <summary>
    /// Visualizer configuration
    /// </summary>
    public class VisualizerConfigurationSection
    {

        /// <summary>
        /// CTOR for the visualizer configuration section
        /// </summary>
        public VisualizerConfigurationSection()
        {
            this.Endpoints = new List<EndpointConfiguration>();
        }

        /// <summary>
        /// Listener configurations
        /// </summary>
        public List<EndpointConfiguration> Endpoints { get; private set; }


    }
}
