using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase
{
    /// <summary>
    /// Knowledgebase service contract
    /// </summary>
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
        void PublishKbArticle(String kbid, Stream data);
    }
}
