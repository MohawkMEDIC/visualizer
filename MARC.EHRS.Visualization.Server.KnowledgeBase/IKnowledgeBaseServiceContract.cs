using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using MARC.EHRS.VisualizationServer.KnowledgeBase.Contract;
using System.Xml;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase
{
    /// <summary>
    /// Knowledgebase service contract
    /// </summary>
    [XmlSerializerFormat]
    [ServiceContract]
    public interface IKnowledgeBaseServiceContract
    {

        /// <summary>
        /// Get a KB article
        /// </summary>
        [WebGet(UriTemplate = "/kb/{kbid}")]
        Stream GetKbArticle(String kbid);

        /// <summary>
        /// Publish a kb article
        /// </summary>
        [WebInvoke(UriTemplate = "/kb/{kbid}", Method = "POST")]
        void PublishKbArticle(String kbid, XmlElement data);

        /// <summary>
        /// Publish a kb articles
        /// </summary>
        [WebInvoke(UriTemplate = "/kb", Method = "POST")]
        void PublishKbArticleBatch(ArticleCollection data);


        /// <summary>
        /// Find articles
        /// </summary>
        [WebGet(UriTemplate = "/kb")]
        ArticleCollection FindArticles();
    }
}
