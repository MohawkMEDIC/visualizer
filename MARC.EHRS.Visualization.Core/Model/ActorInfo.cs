using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Represents extended actor information
    /// </summary>
    public class ActorInfo : AtnaApi.Model.AuditActorData
    {

        /// <summary>
        /// Gets or sets the node identifier
        /// </summary>
        public NodeInfo Node { get; set; }


    }
}
