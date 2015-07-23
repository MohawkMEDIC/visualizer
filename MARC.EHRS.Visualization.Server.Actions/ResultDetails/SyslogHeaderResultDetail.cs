using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.Everest.Connectors;

namespace MARC.EHRS.VisualizationServer.Actions.ResultDetails
{
    /// <summary>
    /// A result detail which represents a syslog header parse error
    /// </summary>
    public class SyslogHeaderResultDetail : ResultDetail
    {

        /// <summary>
        /// Creates a new SyslogHeaderResultDetail
        /// </summary>
        public SyslogHeaderResultDetail(ResultDetailType type, String message, Exception exception) : base(type, message, exception)
        {
        }

    }
}
