using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Data about an audits enterprise site
    /// </summary>
    public class EnterpriseSiteInfo : VersionedData
    {

        /// <summary>
        /// Gets or sets the friendly name of the enterprise site
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets ors sets the external identifier of the site
        /// </summary>
        public List<string> ExternalIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets the status of the site
        /// </summary>
        public StatusType Status { get; set; }

        /// <summary>
        /// Gets or sets the configured sources which are permitted to use the site
        /// </summary>
        public List<EnterpriseSourceInfo> Sources { get; set; }

    }
}
