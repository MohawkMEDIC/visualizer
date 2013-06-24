using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.DataTypes;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Identifies a logical audit source
    /// </summary>
    public class AuditSourceInfo : StoredData
    {

        /// <summary>
        /// Gets or sets the enterprise site information
        /// </summary>
        public EnterpriseSiteInfo EnterpriseSite { get; set; }

        /// <summary>
        /// Gets or sets the logical information about the source
        /// </summary>
        public EnterpriseSourceInfo Source { get; set; }

        /// <summary>
        /// Gets or sets the source types
        /// </summary>
        public List<CodeValue> SourceType { get; set; }

    }
}
