using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.Visualization.Client.Silverlight.Contract.Visualizer;

namespace MARC.EHRS.Visualization.Client.Silverlight
{
    /// <summary>
    /// Event arguments for visualization
    /// </summary>
    public class VisualizationEventArgs : EventArgs
    {
        /// <summary>
        /// The event args
        /// </summary>
        public VisualizationEvent Event { get; set; }

        /// <summary>
        /// Error Text
        /// </summary>
        public String ErrorText { get; set; }
    }
}
