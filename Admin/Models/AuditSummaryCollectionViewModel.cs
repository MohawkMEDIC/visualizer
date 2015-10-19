using Admin.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Models
{
    /// <summary>
    /// Summary collection model
    /// </summary>
    public class AuditSummaryCollectionViewModel 
    {

        /// <summary>
        /// Results of the query
        /// </summary>
        public IQueryable<AuditSummaryVw> Results { get; set; }
    }
}
