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

namespace MARC.EHRS.VisualizationClient.Silverlight.Config
{
    /// <summary>
    /// Configuration has been loaded
    /// </summary>
    public class ConfigurationLoadedEventArgs : EventArgs
    {

        /// <summary>
        /// Gets the configuration that was loaded
        /// </summary>
        public Configuration Configuration { get; set; }
    }
}
