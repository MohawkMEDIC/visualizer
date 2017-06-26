﻿/*
 * Copyright 2012-2017 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2012-6-15
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtnaApi.Model;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using MARC.Everest.Connectors;
using MARC.EHRS.VisualizationServer.Syslog;
using MARC.EHRS.VisualizationServer.Actions.ResultDetails;
using System.Diagnostics;

namespace MARC.EHRS.VisualizationServer.Actions
{
    /// <summary>
    /// Message utilities
    /// </summary>
    public static class MessageUtil
    {

        /// <summary>
        /// Audit parsing operation result
        /// </summary>
        public struct ParseAuditResult
        {

            /// <summary>
            /// Gets the source message
            /// </summary>
            public SyslogMessage SourceMessage { get; internal set; }

            /// <summary>
            /// Gets the details of the parse operation
            /// </summary>
            public IEnumerable<IResultDetail> Details { get; internal set; }

            /// <summary>
            /// Parse result
            /// </summary>
            public ResultCode Outcome { get; internal set; }

            /// <summary>
            /// Gets the parsed message if applicable
            /// </summary>
            public AuditMessage Message { get; internal set; }

        }

        /// <summary>
        /// Parse an audit
        /// </summary>
        public static ParseAuditResult ParseAudit(SyslogMessage message)
        {
            // Deserialize audit message.
            ParseAuditResult retVal = new ParseAuditResult();
            List<IResultDetail> details = new List<IResultDetail>();
            retVal.SourceMessage = message;
            try
            {
                // Is the message invalid?
                if (String.IsNullOrEmpty(message.Body)) // no body == invalid
                {
                    retVal.Outcome = ResultCode.Rejected;
                    details.Add(new SyslogHeaderResultDetail(ResultDetailType.Error, "Could not parse Syslog header", null));
                }
                else
                {
                    string payload = message.Body;
                    if (message.TypeId.Contains("DICOM")) // Dicom
                        payload = AtnaApi.Transport.AuditTransportUtil.ConvertAuditToRFC3881(payload);
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AuditMessage));
                    StringReader strReader = new StringReader(payload.Substring(payload.IndexOf("<Audit")));
                    StringWriter strWriter = new StringWriter();
                    XmlTextReader xmlTextReader = new XmlTextReader(strReader);
                    retVal.Message = xmlSerializer.Deserialize(xmlTextReader) as AuditMessage;
                    retVal.Outcome = ResultCode.Accepted;
                }
            }
            catch (Exception e)
            {

                StringBuilder exceptionBuilder = new StringBuilder(e.Message);
                Exception ie = e.InnerException;
                while (ie != null)
                {
                    exceptionBuilder.AppendFormat(" -> {0} ", ie.Message);
                    ie = ie.InnerException;
                }
                // Add result detail
                details.Add(new Rfc3881ParseResultDetail(ResultDetailType.Error, exceptionBuilder.ToString(), e));
                retVal.Outcome = ResultCode.Error;
            }
            retVal.Details = details;
            return retVal;
        }

    }
}
