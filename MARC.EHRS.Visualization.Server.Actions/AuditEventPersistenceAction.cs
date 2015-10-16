using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.VisualizationServer.Syslog;
using System.Diagnostics;
using MARC.EHRS.Visualization.Core.Model;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using MARC.EHRS.Visualization.Core.Services;
using System.Security.Principal;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MARC.EHRS.VisualizationServer.Actions
{
    /// <summary>
    /// Represents a persistence action that calls an IAuditEventInfoPersistence instance
    /// </summary>
    public class AuditEventPersistenceAction : ISyslogAction
    {

        
        #region ISyslogAction Members
        
        /// <summary>
        /// Handle a message being received by the message handler
        /// </summary>
        public void HandleMessageReceived(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            this.ProcessMessage(e);
        }

        /// <summary>
        /// Handles an invalid message being persisted
        /// </summary>
        public void HandleInvalidMessage(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            this.ProcessMessage(e);
        }

        /// <summary>
        /// Holds an audit message
        /// </summary>
        private void HoldAuditMessage(AuditMessageInfo ami, String reason)
        {
            ami.Status.EffectiveTo = DateTime.Now;
            ami.StatusHistory.Add(new AuditStatusEntry()
            {
                EffectiveFrom = DateTime.Now,
                StatusCode = StatusType.Held,
                SetByUserId = WindowsIdentity.GetCurrent().Name
            });
            ami.Errors.Add(new AuditErrorInfo() {
                Message = reason
            });
        }


        /// <summary>
        /// Process a message received by the syslog message handler
        /// </summary>
        private void ProcessMessage(Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            try
            {
                if (e == null || e.Message == null)
                {
                    Trace.TraceWarning("Received null SyslogEvent from transport");
                    return;
                }


                // Secured copy
                AuthenticatedSyslogMessageReceivedEventArgs securedEvent = e as AuthenticatedSyslogMessageReceivedEventArgs;

                // Node information lookup service
                INodeInfoLookupService nodeService = this.Context.GetService(typeof(INodeInfoLookupService)) as INodeInfoLookupService;
                IAuditPersistenceService storeService = this.Context.GetService(typeof(IAuditPersistenceService)) as IAuditPersistenceService;

                // Process a result
                AuditMessageInfo ami = null;
                var processResult = MessageUtil.ParseAudit(e.Message);

                
                // Is this an error?
                if (processResult.Outcome != Everest.Connectors.ResultCode.Accepted)
                    ami = this.CreateInvalidAuditMessageInfo(processResult);
                else
                {
                    ami = new AuditMessageInfo()
                    {
                        StatusHistory = new List<AuditStatusEntry>()
                    {
                        new AuditStatusEntry()
                        {
                            EffectiveFrom = DateTime.Now,
                            StatusCode = StatusType.New
                        }
                    }
                    };
                    ami.Event = processResult.Message;
                }

                // Set core properties
                ami.CorrelationToken = e.Message.CorrelationId;
                ami.CreationTime = DateTime.Now;
                ami.SessionId = e.Message.SessionId;

                // Sender / receiver information
                Uri solicitorSource = new Uri(String.Format("atna://{0}", e.SolicitorEndpoint.Host)),
                    solicitorHost = new Uri(String.Format("atna://{0}", e.Message.HostName)),
                    receiveHost = new Uri(String.Format("atna://{0}", e.ReceiveEndpoint.Host));

                ami.Receiver = nodeService.GetNodeInfo(receiveHost, false);
                if (ami.Receiver == null)
                {
                    ami.Status.IsAlert = true;
                    ami.Errors.Add(new AuditErrorInfo()
                    {
                        Message = String.Format("Receive endpoint {0} is unknown", e.ReceiveEndpoint)
                    });
                    ami.Receiver = new NodeInfo()
                    {
                        Status = StatusType.New,
                        Name = e.ReceiveEndpoint.ToString(),
                        Host = e.ReceiveEndpoint
                    };
                }


                ami.SenderNode = nodeService.GetNodeInfo(solicitorSource, false);
                if (ami.SenderNode == null && e.Message.HostName != "-")
                    ami.SenderNode = nodeService.GetNodeInfo(solicitorHost, false);

                if (ami.SenderNode == null)
                {
                    ami.Status.IsAlert = true;
                    ami.Errors.Add(new AuditErrorInfo()
                    {
                        Message = String.Format("Solicitor endpoint/hostname {0}/{1} is unknown", solicitorSource, solicitorHost)
                    });
                    ami.SenderNode = new NodeInfo()
                    {
                        Status = StatusType.New,
                        Name = e.Message.HostName,
                        Host = e.Message.HostName != "-" ? solicitorHost : solicitorSource,
                        X509Thumbprint = securedEvent != null ? securedEvent.RemoteCertificate.GetCertHashString() : null
                    };
                }

                // Sender node
                ami.SenderProcess = e.Message.ProcessName;

                if (securedEvent != null && ami.SenderNode.X509Thumbprint != securedEvent.RemoteCertificate.GetCertHashString())
                {
                    ami.Status.IsAlert = true;
                    ami.Errors.Add(new AuditErrorInfo()
                    {
                        Message = String.Format("Cert hash {0} does not equal configured value of {1}", securedEvent.RemoteCertificate.GetCertHashString(), ami.SenderNode.X509Thumbprint)
                    });
                }

                if (storeService != null)
                    storeService.PersistAuditMessage(ami);
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.ToString());
                throw;
            }
        }

        /// <summary>
        /// Create invalid audit message info
        /// </summary>
        private AuditMessageInfo CreateInvalidAuditMessageInfo(MessageUtil.ParseAuditResult processResult)
        {
            // Construct the return value
            var retVal = new AuditMessageInfo()
            {
                StatusHistory = new List<AuditStatusEntry>() {
                    new AuditStatusEntry()
                    {
                        EffectiveFrom = DateTime.Now,
                        IsAlert = true,
                        StatusCode = StatusType.Held
                    }
                }, // Held because message is an error
                Errors = new List<AuditErrorInfo>()
            };

            // Append errors
            foreach (var dtl in processResult.Details)
                retVal.Errors.Add(new AuditErrorInfo(dtl));

            return retVal;
        }

        #endregion

        #region IUsesHostContext Members

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
