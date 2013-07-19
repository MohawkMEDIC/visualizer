using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.EHRS.Visualization.Core.Model;
using System.Security.Cryptography.X509Certificates;

namespace MARC.EHRS.Visualization.Core.Services
{
    /// <summary>
    /// Represents an class that can persist and retrieve NodeInfo and ProcessInfo structures
    /// </summary>
    public interface INodeInfoLookupService : IUsesHostContext
    {

        /// <summary>
        /// Get node information
        /// </summary>
        NodeInfo GetNodeInfo(Uri nodeEndpoint, bool includeHistory);

        /// <summary>
        /// Get node information by id
        /// </summary>
        NodeInfo GetNodeInfo(decimal nodeId, bool includeHistory);

        /// <summary>
        /// Search for node information
        /// </summary>
        List<NodeInfo> SearchNodeInfo(NodeInfo prototype);

    }
}
