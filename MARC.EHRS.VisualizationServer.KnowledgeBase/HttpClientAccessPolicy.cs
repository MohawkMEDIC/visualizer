using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.IO;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Reflection;
using System.Diagnostics;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase
{

    /// <summary>
    /// CAP Server handler
    /// </summary>
    public class CapServerListener : IMessageHandlerService
    {
        // Service host
        private WebServiceHost m_serviceHost;

        #region IMessageHandlerService Members

        /// <summary>
        /// Start the service
        /// </summary>
        public bool Start()
        {
            this.m_serviceHost = new WebServiceHost(typeof(HttpClientAccessPolicyBehavior));
            this.m_serviceHost.Open();
            return true;
        }

        /// <summary>
        /// Stop the service;
        /// </summary>
        public bool Stop()
        {
            if(this.m_serviceHost != null)
                this.m_serviceHost.Close();
            return true;
        }

        #endregion

        #region IUsesHostContext Members

        /// <summary>
        /// Gets or sets the context
        /// </summary>
        public IServiceProvider Context
        {
            get;
            set;
        }

        #endregion
    }

    /// <summary>
    /// CAP Server
    /// </summary>
    [ServiceContract()]
    public interface IHttpClientAccessPolicyContract
    {

        /// <summary>
        /// GEt client access policy
        /// </summary>
        [WebGet(UriTemplate = "/clientaccesspolicy.xml")]
        Stream GetCapFile();

    }

    /// <summary>
    /// CAP Server behavior
    /// </summary>
    public class HttpClientAccessPolicyBehavior : IHttpClientAccessPolicyContract
    {
        #region IHttpClientAccessPolicyContract Members

        /// <summary>
        /// Get the CAP file
        /// </summary>
        public Stream GetCapFile()
        {
            String path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "ClientAccessPolicy.xml");
            try
            {
                using (Stream s = File.OpenRead(path))
                {
                    WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
                    MemoryStream retVal = new MemoryStream();
                    byte[] buffer = new byte[1024];
                    int br = buffer.Length;
                    while (br == buffer.Length)
                    {
                        br = s.Read(buffer, 0, 1024);
                        retVal.Write(buffer, 0, br);
                    }
                    retVal.Seek(0, SeekOrigin.Begin);
                    return retVal;
                }
            }
            catch (FileNotFoundException)
            {
                throw new WebFaultException(System.Net.HttpStatusCode.NotFound);
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        #endregion
    }
}
