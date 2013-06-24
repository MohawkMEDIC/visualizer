using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.DataTypes;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Audit participant info
    /// </summary>
    public class AuditParticipantInfo : StoredData
    {

        /// <summary>
        /// Gets or sets the user identification
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the alternate user id
        /// </summary>
        public string AlternateUserId { get; set; }

        /// <summary>
        /// Gets or sets the node the active participant is/was using
        /// </summary>
        public NodeInfo Node { get; set; }

        /// <summary>
        /// Gets or sets whether the user was a requestor
        /// </summary>
        public Boolean IsRequestor { get; set; }

        /// <summary>
        /// Gets or sets the roles the participant was playing
        /// </summary>
        public List<CodeValue> Roles { get; set; }
    }
}
