using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.DataTypes;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Identifies auditable object information
    /// </summary>
    public class AuditObjectInfo : StoredData
    {

        /// <summary>
        /// Gets or sets the external identifier used for the object
        /// </summary>
        public string ObjectIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the type of object
        /// </summary>
        public CodeValue Type { get; set; }

        /// <summary>
        /// Gets or sets the role of the object
        /// </summary>
        public CodeValue Role { get; set; }

        /// <summary>
        /// Gets or sets the lifecycle of the object
        /// </summary>
        public CodeValue Lifecycle { get; set; }

        /// <summary>
        /// Gets or sets the type of identifier
        /// </summary>
        public CodeValue IdentifierType { get; set; }

        /// <summary>
        /// Gets or sets the object detail
        /// </summary>
        public string ObjectDetail { get; set; }

        /// <summary>
        /// Gets or sets the type which the detail represents
        /// </summary>
        public ObjectDetailType ObjectDetailType { get; set; }

        /// <summary>
        /// Gets or sets the object identification
        /// </summary>
        public List<KeyValuePair<String, Byte[]>> ObjectIdentification { get; set; }
    }
}
