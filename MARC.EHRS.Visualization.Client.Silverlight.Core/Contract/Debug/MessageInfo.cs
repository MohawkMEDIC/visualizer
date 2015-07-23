using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MARC.EHRS.Visualization.Client.Silverlight.Contract.Debug
{
    /// <summary>
    /// Message information
    /// </summary>
    [XmlRoot("message", Namespace = "http://marc-hi.ca/svccore/message")]
    [XmlType("MessageInfo", Namespace = "http://marc-hi.ca/svccore/message")]
    public class MessageInfo
    {
        /// <summary>
        /// The id of the message
        /// </summary>
        [XmlElement("id")]
        public String Id { get; set; }
        /// <summary>
        /// The identifier of the message this message responds to
        /// </summary>
        [XmlElement("response")]
        public MessageInfo Response { get; set; }
        /// <summary>
        /// The destination address of the message
        /// </summary>
        [XmlElement("to")]
        public String To { get; set; }
        /// <summary>
        /// The source address of the message
        /// </summary>
        [XmlElement("from")]
        public String From { get; set; }
        /// <summary>
        /// The date the message was received
        /// </summary>
        [XmlElement("date")]
        public DateTime Timestamp { get; set; }
    }
}
