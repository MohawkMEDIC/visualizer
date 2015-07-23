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

namespace MARC.EHRS.Visualizer.Client.Silverlight.Contract.Debug
{
    /// <summary>
    /// Message collection
    /// </summary>
    [XmlType("MessageCollection", Namespace = "http://marc-hi.ca/svccore/message")]
    [XmlRoot("messages", Namespace = "http://marc-hi.ca/svccore/message")]
    public class MessageCollection
    {

        /// <summary>
        /// Gets the messages
        /// </summary>
        [XmlElement("message")]
        public List<MessageInfo> Messages { get; set; }
    }
}
