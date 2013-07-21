using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase.Contract
{
    /// <summary>
    /// Article collection
    /// </summary>
    [XmlRoot("ArticleCollection")]
    [XmlType("ArticleCollection")]
    public class ArticleCollection 
    {

        /// <summary>
        /// Article collection ctor
        /// </summary>
        public ArticleCollection()
        {
            this.Article = new List<Article>();
        }

        /// <summary>
        /// The list of articles contained in this collection
        /// </summary>
        [XmlElement("Article")]
        public List<Article> Article { get; set; }

    }
}
