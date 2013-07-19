using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.EHRS.VisualizationServer.KnowledgeBase.Configuration;
using System.Configuration;
using System.ServiceModel.Web;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase
{
    /// <summary>
    /// Message listener for the KB service
    /// </summary>
    public class KnowledgeBaseListener : IMessageHandlerService
    {

        // Configuration handler
        private static ConfigurationHandler s_handler;
        // Service host
        private WebServiceHost m_serviceHost;

        /// <summary>
        /// Get the configuraiton handler
        /// </summary>
        public static ConfigurationHandler Configuration { get { return s_handler; } }

        /// <summary>
        /// Get configuration
        /// </summary>
        static KnowledgeBaseListener()
        {
            s_handler = ConfigurationManager.GetSection("marc.ehrs.visualizationserver.knowledgebase") as ConfigurationHandler;
        }

        #region IMessageHandlerService Members

        /// <summary>
        /// Start the service
        /// </summary>
        public bool Start()
        {
            this.m_serviceHost = new WebServiceHost(typeof(KnowledgeBaseServiceBehavior));
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
}
