using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using MARC.EHRS.VisualizationServer.Syslog.Exceptions;

namespace MARC.EHRS.VisualizationServer.Syslog
{
    /// <summary>
    /// Represents a raw syslog message
    /// </summary>
    public class SyslogMessage
    {
        /// <summary>
        /// Gets or sets the facility
        /// </summary>
        public int Facility { get; set; }

        /// <summary>
        /// Gets or sets the version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the host name
        /// </summary>
        public String HostName { get; set; }

        /// <summary>
        /// Gets or sets the process name
        /// </summary>
        public String ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the process id
        /// </summary>
        public decimal ProcessId { get; set; }

        /// <summary>
        /// The body of the syslog message
        /// </summary>
        public byte[] Body { get; set; }

        /// <summary>
        /// Parse the syslog message
        /// </summary>
        public static SyslogMessage Parse(String message)
        {
            SyslogMessage retVal = new SyslogMessage();

            // Process the message... First remove newlines
            message = message.Replace("\r\n", "");
            // Now regex
            Regex re = new Regex(@"^\<(\d+)\>(\d+)\s([\dTZ\:\.\-]*)\s([\w\.\-]*)\s([\w\-\.]*)\s([\d\-]*)\s([\w\+\-\.]*)\s([\w\+\-\.]*)(.*)$");
            Match match = re.Match(message);

            if (match.Success)
            {
                // Get the properties and log
                ;
            }
            else
            {
                retVal.Body = Encoding.UTF8.GetBytes(message);
                throw new SyslogMessageException("Invalid message format", retVal);
            }

            return retVal;
        }
    }
}
