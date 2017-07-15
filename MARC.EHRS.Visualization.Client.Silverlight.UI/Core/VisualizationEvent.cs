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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MARC.EHRS.Visualization.Client.Silverlight.UI.Core
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
    }
}
