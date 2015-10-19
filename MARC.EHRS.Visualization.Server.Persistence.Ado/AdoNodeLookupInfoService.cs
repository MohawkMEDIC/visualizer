using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.Visualization.Core.Services;
using MARC.EHRS.Visualization.Core.Model;

namespace MARC.EHRS.Visualization.Server.Persistence.Ado
{
    /// <summary>
    /// Represents a service that can lookup node information 
    /// </summary>
    public class AdoNodeLookupInfoService : INodeInfoLookupService
    {
        /// <summary>
        /// Context
        /// </summary>
        public IServiceProvider Context { get; set; }

        #region INodeInfoLookupService Members

        /// <summary>
        /// Get node information given the endpoint of the node
        /// </summary>
        public Visualization.Core.Model.NodeInfo GetNodeInfo(Uri nodeEndpoint, bool includeHistory)
        {
            using (var context = new AuditModelDataContext())
            {
                var nodeVersion = context.NodeVersions.Where(o => o.HostName == nodeEndpoint.Host && o.ObsoletionTime == null).OrderByDescending(o => o.NodeVersionId).FirstOrDefault();

                if (nodeVersion != null)
                    return AdoAuditPersistenceService.ParseNodeVersion(nodeVersion);

                // TODO: Handle history

                return null;
            }

        }

        /// <summary>
        /// Get node information given the identifier of the node
        /// </summary>
        public Visualization.Core.Model.NodeInfo GetNodeInfo(decimal nodeId, bool includeHistory)
        {
            using (var context = new AuditModelDataContext())
            {
                var nodeVersion = context.NodeVersions.Where(o => o.NodeId == nodeId && o.ObsoletionTime == null).OrderByDescending(o => o.NodeVersionId).FirstOrDefault();

                if (nodeVersion != null)
                    return AdoAuditPersistenceService.ParseNodeVersion(nodeVersion);

                // TODO: Handle history 
            }
            return null;
        }

        /// <summary>
        /// Search for nodes with characteristics matching <paramref name="prototype"/>
        /// </summary>
        public List<Visualization.Core.Model.NodeInfo> SearchNodeInfo(Visualization.Core.Model.NodeInfo prototype)
        {
            using (var context = new AuditModelDataContext())
            {
                if (prototype.Id != default(int))
                    return new List<NodeInfo>() { this.GetNodeInfo(prototype.Id, false) };
                else if (prototype.Host != null)
                    return new List<NodeInfo>() { this.GetNodeInfo(prototype.Host, false) };
                else
                    return context.NodeVersions.Where(o => o.NodeMagic == Convert.FromBase64String(prototype.X509Thumbprint)).Select(o => AdoAuditPersistenceService.ParseNodeVersion(o)).ToList();
            }
        }

       
        #endregion
    }
}
