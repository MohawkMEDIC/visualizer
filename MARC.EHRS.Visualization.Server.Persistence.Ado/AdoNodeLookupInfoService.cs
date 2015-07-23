using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.Visualization.Core.Services;

namespace MARC.EHRS.VisualizationServer.Actions.Persistence
{
    /// <summary>
    /// Represents a service that can lookup node information 
    /// </summary>
    public class AdoNodeLookupInfoService : INodeInfoLookupService
    {
        #region INodeInfoLookupService Members

        /// <summary>
        /// Get node information given the endpoint of the node
        /// </summary>
        public Visualization.Core.Model.NodeInfo GetNodeInfo(Uri nodeEndpoint, bool includeHistory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get node information given the identifier of the node
        /// </summary>
        public Visualization.Core.Model.NodeInfo GetNodeInfo(decimal nodeId, bool includeHistory)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search for nodes with characteristics matching <paramref name="prototype"/>
        /// </summary>
        public List<Visualization.Core.Model.NodeInfo> SearchNodeInfo(Visualization.Core.Model.NodeInfo prototype)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IUsesHostContext Members

        public IServiceProvider Context
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
