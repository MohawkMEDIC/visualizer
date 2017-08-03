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
using MARC.EHRS.VisualizationServer.Syslog;
using MARC.EHRS.Visualization.Core;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Xml.Serialization;
using System.IO;
using AtnaApi.Model;
using System.Xml;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.EHRS.Visualization.Core.Services;
using System.Threading;

namespace MARC.EHRS.VisualizationServer.Actions
{
    /// <summary>
    /// Represents a syslog action
    /// </summary>
    /// <remarks>Action that will forward audits to clients</remarks>
    public class NotifyAction : ISyslogAction
    {
        #region ISyslogAction Members

        /// <summary>
        /// Sequence
        /// </summary>
        private long m_messageSequence = 0;

        /// <summary>
        /// Converts an audit message to a log message.
        /// </summary>
        /// <param name="message">Audit Message in serialized string format.</param>
        /// <returns>A string serialized format of a log message.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private VisualizationEvent ConvertToVisualization(SyslogMessageReceivedEventArgs evt)
        {
            VisualizationEvent logMessage = null;

            // Clean the message
            var payload = Regex.Replace(evt.Message.Body, "[&]([^a][^m][^p])", "&amp;$1");

            try
            {
                var parseResult = MessageUtil.ParseAudit(evt.Message);
                var auditMessage = parseResult.Message;

                // Create a LogMessage equivalent.
                if (parseResult.Outcome != Everest.Connectors.ResultCode.Accepted)
                {
                    foreach (var itm in parseResult.Details)
                        Trace.TraceError("{0} : {1}", itm.Type, itm.Message);
                    throw new InvalidOperationException("Cannot continue : Audit message is invalid");
                }
                else if (auditMessage != null && auditMessage.SourceIdentification.Count > 0)
                {
                    // Use the Enterprise site ID as the audit source ID.
                    // If not available, use the usual audit source ID.
                    string sourceID = "";

                    sourceID = auditMessage.SourceIdentification[0].AuditEnterpriseSiteID;
                    if (String.IsNullOrEmpty(sourceID))
                    {
                        sourceID = auditMessage.SourceIdentification[0].AuditSourceID;
                        if (sourceID.Contains("@"))
                            sourceID = sourceID.Substring(0, sourceID.IndexOf("@"));
                        Trace.TraceInformation("Using audit source id: {0}", sourceID);
                    }
                    else if (!String.IsNullOrEmpty(auditMessage.SourceIdentification[0].AuditSourceID))
                    {
                        sourceID = auditMessage.SourceIdentification[0].AuditSourceID + "^" + sourceID;
                        Trace.TraceInformation("Using audit source id and enterprise site id: {0}", sourceID);
                    }
                    else
                        Trace.TraceInformation("Using enterprise site id: {0}", sourceID);


                    // Create the log message
                    logMessage = new VisualizationEvent()
                    {
                        MachineOID = sourceID,
                        CorrelationId = evt.Message.CorrelationId.ToString(),
                        Sequence = Interlocked.Increment(ref this.m_messageSequence)
                    };
                    if (evt.SolicitorEndpoint != null)
                        logMessage.IPAddress = evt.SolicitorEndpoint.ToString();
                    if (auditMessage.EventIdentification != null)
                        logMessage.TimeStamp = auditMessage.EventIdentification.EventDateTime;
                    if (auditMessage.EventIdentification.EventType != null &&
                        auditMessage.EventIdentification.EventType.Count > 0)
                        logMessage.EventType = auditMessage.EventIdentification.EventType[0].Code;
                    if (auditMessage.EventIdentification != null && auditMessage.EventIdentification.EventId != null)
                        logMessage.EventID = auditMessage.EventIdentification.EventId.Code.ToString();
                    // Try to resolve the OID
                    ISystemConfigurationService configService = Context.GetService(typeof(ISystemConfigurationService)) as ISystemConfigurationService;
                    if (configService != null)
                    {
                        var data = configService.OidRegistrar.FindData(logMessage.MachineOID);
                        if (data != null)
                            logMessage.Name = data.Description;
                    }

                }
                else
                {
                    Trace.TraceWarning("Missing SourceIdentification, will use Syslog processName: {0}", evt.Message.ProcessName);
                    // Create the log message
                    logMessage = new VisualizationEvent()
                    {
                        MachineOID = evt.Message.ProcessName,
                        CorrelationId = evt.Message.CorrelationId.ToString(),
                        Sequence = Interlocked.Increment(ref this.m_messageSequence)
                    };
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

                Trace.TraceError("{0} : {1}", exceptionBuilder, payload);
                Trace.TraceError(e.ToString());
                logMessage = new VisualizationEvent()
                {
                    IsError = true,
                    CorrelationId = evt.Message.CorrelationId.ToString(),
                    Sequence = Interlocked.Increment(ref this.m_messageSequence),
                    MachineOID = evt.Message.ProcessName,
                    IPAddress = evt.SolicitorEndpoint.Host,
                    TimeStamp = DateTime.Now
                };
            }

            return logMessage;
        }

        /// <summary>
        /// Handle a message being received
        /// </summary>
        public void HandleMessageReceived(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            try
            {
                if (e == null || e.Message == null)
                    return; // no message

                // Notify received
                Trace.TraceInformation("Received message from {0} on {2} with correlation id {1}", e.SolicitorEndpoint, e.Message.CorrelationId, e.ReceiveEndpoint);
                var evt = this.ConvertToVisualization(e);
                if (evt != null)
                {
                    INotificationService notif = Context.GetService(typeof(INotificationService)) as INotificationService;
                    if (notif == null)
                        Trace.TraceError("Cannot find a registered broadcaster");
                    else
                        notif.Notify(evt);
                }
                else
                    Trace.TraceError("Could not create visualization event");
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        /// <summary>
        /// Handle an invalid message being received
        /// </summary>
        public void HandleInvalidMessage(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            try
            {
                if (e == null || e.Message == null)
                    return; // no message

                // Notify received
                Trace.TraceInformation("Received invalid message from {0} (Invalid Header) : {1}", e.SolicitorEndpoint, e.Message.Body);
                INotificationService notif = Context.GetService(typeof(INotificationService)) as INotificationService;
                var evt = new VisualizationEvent()
                {
                    CorrelationId = e.Message.CorrelationId.ToString(),
                    MachineOID = e.Message.ProcessName ?? e.SolicitorEndpoint.Host,
                    SrcPort = e.SolicitorEndpoint.Port.ToString(),
                    IPAddress = e.SolicitorEndpoint.Host
                };
                if (notif == null)
                    Trace.TraceError("Cannot find a registered broadcaster");
                else
                    notif.Notify(evt);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        /// <summary>
        /// Gets or sets the host context
        /// </summary>
        public IServiceProvider Context
        {
            get;
            set;
        }

        #endregion
    }
}