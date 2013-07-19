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
            ami.Status = StatusType.Held;
            ami.Errors.Add(new AuditErrorInfo() {
                Message = reason
            });
        }

        /// <summary>
        /// Principal
        /// </summary>
        private byte[] SerializePrincipal(WindowsIdentity principal)
        {
            using(MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, principal);
                ms.Seek(0, SeekOrigin.Begin);

                // Read
                byte[] retVal = new byte[ms.Length];
                ms.Read(retVal, 0, (int)ms.Length);
                return retVal;
            }
        }

        /// <summary>
        /// Process a message received by the syslog message handler
        /// </summary>
        private void ProcessMessage(Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            if (e == null || e.Message == null)
            {
                Trace.TraceWarning("Received null SyslogEvent from transport");
                return;
            }


            // Secured copy
            AuthenticatedSyslogMessageReceivedEventArgs securedEvent = e as AuthenticatedSyslogMessageReceivedEventArgs;

            // Node information lookup service
            INodeInfoLookupService nodeInfo = this.Context.GetService(typeof(INodeInfoLookupService)) as INodeInfoLookupService;

            // Process a result
            var processResult = MessageUtil.ParseAudit(e.Message);
            
            AuditMessageInfo ami = null;
            // Is this an error?
            if (processResult.Outcome != Everest.Connectors.ResultCode.Accepted)
                ami = this.CreateInvalidAuditMessageInfo(processResult);
            else
                ; // TODO: Serialize the full message
            // Principal 
            ami.CreatorPrincipalData = this.SerializePrincipal(WindowsIdentity.GetCurrent());
            
            // Sender / receiver information
            ami.Receiver = nodeInfo.GetNodeInfo(e.ReceiveEndpoint, false);
            ami.SenderNode = nodeInfo.GetNodeInfo(e.SolicitorEndpoint, false);

            // Sender node
            if(ami.SenderNode != null)
                ami.SenderProcess = ami.SenderNode.Processes.Find(o => o.Name == e.Message.ProcessName);



        }

        /// <summary>
        /// Create invalid audit message info
        /// </summary>
        private AuditMessageInfo CreateInvalidAuditMessageInfo(MessageUtil.ParseAuditResult processResult)
        {
            // Construct the return value
            var retVal = new AuditMessageInfo()
            {
                CorrelationToken = processResult.SourceMessage.CorrelationId,
                CreationTime = processResult.SourceMessage.Timestamp,
                Status = StatusType.Held, // Held because message is an error
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
