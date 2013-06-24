using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Audit session information
    /// </summary>
    public class AuditSessionInfo : StoredData
    {

        /// <summary>
        /// Gets or sets the start of the session
        /// </summary>
        public DateTime SessionStart { get; set; }

    }
}
