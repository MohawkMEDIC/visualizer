using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Represents a status entry
    /// </summary>
    public class AuditStatusEntry
    {

        /// <summary>
        /// Represetns the status code
        /// </summary>
        public StatusType StatusCode { get; set; }
        /// <summary>
        /// Represents the effective time
        /// </summary>
        public DateTime EffectiveFrom { get; set; }
        /// <summary>
        /// Represents the obsoletion time
        /// </summary>
        public DateTime EffectiveTo { get; set; }
        /// <summary>
        /// True if the status condition is an alert
        /// </summary>
        public bool IsAlert { get; set; }
        /// <summary>
        /// Set by the user identifier
        /// </summary>
        public String SetByUserId { get; set; }
    }
}
