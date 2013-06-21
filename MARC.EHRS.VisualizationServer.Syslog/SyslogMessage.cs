﻿using System;
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
        /// Constructor
        /// </summary>
        public SyslogMessage()
        {
            this.CorrelationId = Guid.NewGuid();
        }

        /// <summary>
        /// Gets the unique identifier that can be used to correlate this message
        /// </summary>
        public Guid CorrelationId { get; private set; }

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
        /// Type id of the message
        /// </summary>
        public string TypeId { get; set; }

        /// <summary>
        /// Gets or sets the process name
        /// </summary>
        public String ProcessName { get; set; }

        /// <summary>
        /// Gets or sets the process id
        /// </summary>
        public String ProcessId { get; set; }

        /// <summary>
        /// The body of the syslog message
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Parse the syslog message
        /// </summary>
        public static SyslogMessage Parse(String message)
        {
            SyslogMessage retVal = new SyslogMessage();

            // Process the message... First remove newlines
            message = message.Replace("\r\n", "");
            // Now regex
            Regex re = new Regex(@"^\<(\d+)\>(\d+)\s([\dTZ\:\.\-]*)\s([\w\.\-\@]*)\s([\w\-\.\@]*)\s([\d\-\@]*)\s([\w\+\-\.\@]*)\s([\w\+\-\.\@]*)(.*)$");
            Match match = re.Match(message);

            if (match.Success)
            {
                // Get the properties and log
                try
                {
                    retVal.Facility = Int32.Parse(match.Groups[1].Value);
                    retVal.Version = Int32.Parse(match.Groups[2].Value);
                    retVal.Timestamp = DateTime.Parse(match.Groups[3].Value);
                    retVal.HostName = match.Groups[4].Value;
                    retVal.ProcessName = match.Groups[5].Value;
                    retVal.ProcessId = match.Groups[6].Value;
                    retVal.TypeId = match.Groups[7].Value;
                    retVal.Body = match.Groups[9].Value;
                }
                catch(Exception e)
                {
                    throw new SyslogMessageException("Error processing message", retVal, e);
                }
            }
            else
            {
                retVal.Body = message;
                throw new SyslogMessageException("Invalid message format", retVal);
            }

            return retVal;
        }
    }
}
