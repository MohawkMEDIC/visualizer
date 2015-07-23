using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MARC.EHRS.Visualization.Client.Silverlight.Contract.Visualizer
{
    /// <summary>
    /// Identifies an event has occurred in regards to a visualization
    /// </summary>
    [XmlRoot("evt")]
    public class VisualizationEvent
    {
        /// <summary>
        /// Gets or sets the time that the event occurred
        /// </summary>
        [XmlElement("ts")]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the machine oid that the event occurred on
        /// </summary>
        [XmlElement("oid")]
        public string MachineOID { get; set; }

        /// <summary>
        /// Gets or sets the machine oid that the event occurred on
        /// </summary>
        [XmlElement("ip")]
        public string IPAddress { get; set; }

        /// <summary>
        /// A descriptive name about the event
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// The sequence of the log item
        /// </summary>
        [XmlAttribute("sequence")]
        public decimal Sequence { get; set; }

        /// <summary>
        /// Gets or sets a custom representation of the image
        /// </summary>
        [XmlElement("customRepresentation")]
        public string CustomRepresentation { get; set; }

        /// <summary>
        /// Event identifier
        /// </summary>
        [XmlAttribute("eventId")]
        public string EventID { get; set; }

        /// <summary>
        /// Event type identifier
        /// </summary>
        [XmlAttribute("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// Source port
        /// </summary>
        [XmlAttribute("src")]
        public string SrcPort { get; set; }

        /// <summary>
        /// Correlation ID of the message (used to fetch against the visualization service if needed)
        /// </summary>
        [XmlAttribute("id")]
        public String CorrelationId { get; set; }

        /// <summary>
        /// True if the message was invalid
        /// </summary>
        [XmlAttribute("err")]
        public bool IsError { get; set; }
    }
}
