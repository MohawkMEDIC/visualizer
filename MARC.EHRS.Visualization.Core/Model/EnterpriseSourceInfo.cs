using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Represents logical data about an enterprise source
    /// </summary>
    public class EnterpriseSourceInfo : VersionedData
    {

        /// <summary>
        /// Gets or sets the name of the source 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the expected external identifier
        /// </summary>
        public List<String> ExternalIdentifiers { get; set; }

        /// <summary>
        /// Gets or sets the configured physical node
        /// </summary>
        public NodeInfo Node { get; set; }

        /// <summary>
        /// Gets or sets the configured physical process
        /// </summary>
        public ProcessInfo Process { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public StatusType Status { get; set; }


    }
}
