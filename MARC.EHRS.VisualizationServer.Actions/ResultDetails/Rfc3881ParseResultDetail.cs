using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.Everest.Connectors;

namespace MARC.EHRS.VisualizationServer.Actions.ResultDetails
{
    /// <summary>
    /// A result detail which represents an error parsing the RFC-3881 message
    /// </summary>
    public class Rfc3881ParseResultDetail : ResultDetail
    {

        /// <summary>
        /// Creates a new SyslogHeaderResultDetail
        /// </summary>
        public Rfc3881ParseResultDetail(ResultDetailType type, String message, Exception exception)
            : base(type, message, exception)
        {
        }

    }
}
