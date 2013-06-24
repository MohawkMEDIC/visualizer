using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Contains information about a process
    /// </summary>
    public class ProcessInfo : VersionedData
    {

        /// <summary>
        /// Gets or sets a friendly name for the process
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the process executable name
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the parent processes
        /// </summary>
        public ProcessInfo Parent { get; set; }

        /// <summary>
        /// Gets or sets the current status of the process
        /// </summary>
        public StatusType Status { get; set; }

    }
}
