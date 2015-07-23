using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Xml;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase.Contract
{
    /// <summary>
    /// Article
    /// </summary>
    [XmlType("Article")]
    public class Article
    {

        /// <summary>
        /// Knowledgebas id
        /// </summary>
        [XmlAttribute("kbid")]
        public string KbId { get; set; }

        /// <summary>
        /// Content type
        /// </summary>
        [XmlAttribute("contentType")]
        public string Type { get; set; }

        /// <summary>
        /// XAML
        /// </summary>
        [XmlAnyElement]
        public XmlElement Xaml { get; set; }
    }
}
