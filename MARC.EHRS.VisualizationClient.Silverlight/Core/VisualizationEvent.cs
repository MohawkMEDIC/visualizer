using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MARC.EHRS.Visualization.Core
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
        /// A descriptive name about the event
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// The sequence of the log item
        /// </summary>
        [XmlAttribute("sequence")]
        public long Sequence { get; set; }

        /// <summary>
        /// Gets or sets a custom representation of the image
        /// </summary>
        [XmlElement("customRepresentation")]
        public string CustomRepresentation { get; set; }

        /// <summary>
        /// The time this event was captured
        /// </summary>
        [XmlAttribute("captureTime")]
        public DateTime CapturedAt { get; set; }

        /// <summary>
        /// Event identifier
        /// </summary>
        [XmlAttribute("eventId")]
        public string EventID { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        [XmlAttribute("eventType")]
        public string EventType { get; set; }

        /// <summary>
        /// IP address
        /// </summary>
        [XmlElement("ip")]
        public string Ip { get; set; }
    }
}
