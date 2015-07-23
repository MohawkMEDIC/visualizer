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
using System.Collections.Generic;

namespace MARC.EHRS.Visualizer.Client.Silverlight.Contract.Visualizer
{
    /// <summary>
    /// Visualization collection
    /// </summary>
    [XmlRoot("visualization")]
    public class VisualizationCollection
    {

        [XmlIgnore]
        public bool IsSaved { get; set; }

        [XmlElement("event")]
        public List<VisualizationEvent> Events { get; set; }

        public VisualizationCollection()
        {
            this.Events = new List<VisualizationEvent>(10);
        }
    }
}
