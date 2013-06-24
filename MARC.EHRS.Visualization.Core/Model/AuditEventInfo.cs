using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.DataTypes;
using MARC.HI.EHRS.SVC.Auditing.Atna.Format;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Audit event information
    /// </summary>
    public class AuditEventInfo : StoredData
    {

        /// <summary>
        /// The action of the audit
        /// </summary>
        public CodeValue Action { get; set; }

        /// <summary>
        /// Gets or sets the outcome of the event
        /// </summary>
        public CodeValue Outcome { get; set; }

        /// <summary>
        /// Gets or sets the event identification
        /// </summary>
        public CodeValue EventId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the event
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the event type
        /// </summary>
        public List<CodeValue> EventType { get; set; }

        /// <summary>
        /// Gets or sets the audit source information
        /// </summary>
        public List<AuditSourceInfo> Source { get; set; }

        /// <summary>
        /// Gets or sets the audit participant information
        /// </summary>
        public List<AuditParticipantInfo> Participants { get; set; }

        /// <summary>
        /// Gets or sets the auditable objects
        /// </summary>
        public List<AuditObjectInfo> Objects { get; set; }
    }
}
