using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services;
using AtnaApi.Model;
using MARC.EHRS.Visualization.Core.Model;

namespace MARC.EHRS.Visualization.Core.Services
{

    /// <summary>
    /// Event arguments for persistence events
    /// </summary>
    public class AuditPersistenceEventArgs : EventArgs
    {

        /// <summary>
        /// Creates a new audit persistence event
        /// </summary>
        public AuditPersistenceEventArgs(AuditMessageInfo data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets the data related to the persistence
        /// </summary>
        public AuditMessageInfo Data { get; private set; }

        /// <summary>
        /// When set to true signals a cancel
        /// </summary>
        public bool Cancel { get; set; }
    }

    /// <summary>
    /// Represents a service that can persist and de-persist audit messages
    /// </summary>
    public interface IAudtPersistenceService : IUsesHostContext
    {

        /// <summary>
        /// Persists the audit message
        /// </summary>
        void PersistAuditMessage(AuditMessageInfo audit);

        /// <summary>
        /// De-persists an audit message
        /// </summary>
        AuditMessageInfo DePersistAuditMessage(Guid correlationToken);

        /// <summary>
        /// Search audit messages for those matching the prototype
        /// </summary>
        IEnumerable<AuditMessageInfo> SearchAuditMessage(AuditMessageInfo prototype);

        /// <summary>
        /// Fired prior to persisting of event
        /// </summary>
        event EventHandler<AuditPersistenceEventArgs> Persisting;

        /// <summary>
        /// Fired after persisting
        /// </summary>
        event EventHandler<AuditPersistenceEventArgs> Persisted;

        /// <summary>
        /// Fired when a record is de-persisted
        /// </summary>
        event EventHandler<AuditPersistenceEventArgs> DePersisted;

    }
}
