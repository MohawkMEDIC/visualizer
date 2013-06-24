using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.Everest.Connectors;

namespace MARC.EHRS.Visualization.Core.Model
{
    /// <summary>
    /// Represents an audit error information
    /// </summary>
    public class AuditErrorInfo : StoredData
    {

        /// <summary>
        /// Default ctor
        /// </summary>
        public AuditErrorInfo()
        {

        }

        /// <summary>
        /// Create an audit error info from exception
        /// </summary>
        public AuditErrorInfo(Exception e)
        {
            this.Message = e.Message;
            this.StackTrace = e.StackTrace;
            if (e.InnerException != null)
                this.CausedBy = new AuditErrorInfo(e.InnerException);
        }

        /// <summary>
        /// Create audit error info from result detail
        /// </summary>
        public AuditErrorInfo(IResultDetail dtl)
        {
            this.Message = dtl.Message;
            this.StackTrace = dtl.Location;
            if (dtl.Exception != null)
                this.CausedBy = new AuditErrorInfo(dtl.Exception);
        }

        /// <summary>
        /// Gets or sets the textual message
        /// </summary>
        public String Message { get; set; }

        /// <summary>
        /// Gets or sets the stack trace
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// Gets the error that caused this
        /// </summary>
        public AuditErrorInfo CausedBy { get; set; }

    }
}
