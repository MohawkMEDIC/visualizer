using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Represents a node on the network
    /// </summary>
    public class NodeInfo : StoredData
    {

        /// <summary>
        /// Gets or sets the name of the node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the configured host
        /// </summary>
        public Uri Host { get; set; }

        /// <summary>
        /// Gets or sets the X509 certificate thumbprint
        /// </summary>
        public string X509Thumbprint { get; set; }

        /// <summary>
        /// The node which is the grouping node for this node entry
        /// </summary>
        public NodeInfo GroupNode { get; set; }

        /// <summary>
        /// The status of the node
        /// </summary>
        public StatusType Status { get; set; }


    }
}
