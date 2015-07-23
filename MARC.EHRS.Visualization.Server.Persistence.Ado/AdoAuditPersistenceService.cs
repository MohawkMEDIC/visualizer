using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.VisualizationServer.Actions.Persistence
{
    /// <summary>
    /// Represents an ADO Audit persistence service
    /// </summary> 
    public class AdoAuditPersistenceService : MARC.EHRS.Visualization.Core.Services.IAudtPersistenceService
    {
        #region IAudtPersistenceService Members

        /// <summary>
        /// Persists an audit message
        /// </summary>
        public void PersistAuditMessage(Visualization.Core.Model.AuditMessageInfo audit)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Retrieves an audit message
        /// </summary>
        public Visualization.Core.Model.AuditMessageInfo DePersistAuditMessage(Guid correlationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches for an audit message
        /// </summary>
        public IEnumerable<Visualization.Core.Model.AuditMessageInfo> SearchAuditMessage(Visualization.Core.Model.AuditMessageInfo prototype)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fired when the message is persisting
        /// </summary>
        public event EventHandler<Visualization.Core.Services.AuditPersistenceEventArgs> Persisting;

        /// <summary>
        /// Fired when the message has been persisted
        /// </summary>
        public event EventHandler<Visualization.Core.Services.AuditPersistenceEventArgs> Persisted;

        /// <summary>
        /// Fired when a message has been depersisted
        /// </summary>
        public event EventHandler<Visualization.Core.Services.AuditPersistenceEventArgs> DePersisted;

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
