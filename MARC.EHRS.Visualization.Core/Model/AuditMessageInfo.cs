using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Auditing.Atna.Format;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Audit event information
    /// </summary>
    public class AuditMessageInfo : VersionedData
    {

        /// <summary>
        /// Gets or sets the correlation token
        /// </summary>
        public Guid CorrelationToken { get; set; }

        /// <summary>
        /// Gets or sets the audit session
        /// </summary>
        public AuditSessionInfo Session { get; set; }

        /// <summary>
        /// Gets or sets the sending node
        /// </summary>
        public NodeInfo SenderNode { get; set; }

        /// <summary>
        /// Gets or sets the sender process
        /// </summary>
        public ProcessInfo SenderProcess { get; set; }

        /// <summary>
        /// Gets or sets the receiver node
        /// </summary>
        public NodeInfo Receiver { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public StatusType Status { get; set; }

        /// <summary>
        /// Gets or sets the audit error information
        /// </summary>
        public List<AuditErrorInfo> Errors { get; set; }

        /// <summary>
        /// The processed audit event
        /// </summary>
        public AuditEventInfo Event { get; set; }
    }
}
