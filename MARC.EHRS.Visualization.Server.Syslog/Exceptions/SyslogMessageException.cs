using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.VisualizationServer.Syslog.Exceptions
{
    /// <summary>
    /// Syslog message processing exception
    /// </summary>
    public class SyslogMessageException : Exception
    {

        /// <summary>
        /// Gets the message at fault
        /// </summary>
        public SyslogMessage FaultingMessage { get; private set; }

        /// <summary>
        /// Syslog message exception
        /// </summary>
        public SyslogMessageException(String message) : base(message) { }

        /// <summary>
        /// Syslog messaging exception
        /// </summary>
        public SyslogMessageException(String message, Exception innerException) : base(message) { }

        /// <summary>
        /// Syslog messaging exception
        /// </summary>
        public SyslogMessageException(String message, SyslogMessage faultingMessage) : base(message)
        {
            this.FaultingMessage = faultingMessage;
        }

        public SyslogMessageException(string message, SyslogMessage faultingMessage, Exception innerException) : base(message, innerException)
        {
            this.FaultingMessage = faultingMessage;
        }
    }
}
