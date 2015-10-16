using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Identifies the states of an object
    /// </summary>
    public enum StatusType
    {
        New,
        Active,
        Held, 
        Nullified,
        Obsolete,
        Archived,
        System
    }
}
