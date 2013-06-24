using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Principal;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Versioned data
    /// </summary>
    public abstract class VersionedData : StoredData
    {

        /// <summary>
        /// Gets or sets the version of the data
        /// </summary>
        public decimal VersionId { get; set; }

        /// <summary>
        /// Gets or sets the principal data of the person or machine that created the data
        /// </summary>
        public byte[] CreatorPrincipalData { get; set; }

        /// <summary>
        /// The time the data was created
        /// </summary>
        public DateTime CreationTime { get; set; }

        /// <summary>
        /// When populated, the time the record was obsoleted
        /// </summary>
        public DateTime? ObsoleteTime { get; set; }

        /// <summary>
        /// Gets or sets the versioned data that this object replaces
        /// </summary>
        VersionedData Replaces { get; set; }
    }
}
