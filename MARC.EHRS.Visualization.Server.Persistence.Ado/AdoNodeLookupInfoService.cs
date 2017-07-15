/*
 * Copyright 2012-2017 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2012-6-15
 */
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
