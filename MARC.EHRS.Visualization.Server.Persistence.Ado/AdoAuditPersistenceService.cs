using AtnaApi.Model;
using MARC.EHRS.Visualization.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.Visualization.Core.Model;
using System.Xml.Serialization;
using System.Diagnostics;
using System.Data.SqlClient;

namespace MARC.EHRS.Visualization.Server.Persistence.Ado
{
    /// <summary>
    /// Represents an ADO Audit persistence service
    /// </summary> 
    public class AdoAuditPersistenceService : MARC.EHRS.Visualization.Core.Services.IAuditPersistenceService
    {
        #region IAudtPersistenceService Members

        /// <summary>
        /// Audit context
        /// </summary>
        private AuditModelDataContext m_context;

        /// <summary>
        /// Persists an audit message
        /// </summary>
        public void PersistAuditMessage(Visualization.Core.Model.AuditMessageInfo audit)
        {
            // First we allow callers to change the audit event before we persist
            AuditPersistenceEventArgs evt = new AuditPersistenceEventArgs(audit);
            if (this.Persisting != null)
                this.Persisting(this, evt);

            if (evt.Cancel)
                throw new OperationCanceledException("Persistence of audit cancelled by module hook");

            audit.Status.IsAlert = evt.Alert;

            // Next we want to construct the session
            AuditSession saveSession = this.m_context.AuditSessions.FirstOrDefault(o => o.SessionId == audit.CorrelationToken);
            // No session found? we'll create one
            if (saveSession == null)
            {
                saveSession = new AuditSession()
                {
                    SessionId = audit.SessionId,
                    CreationTimestamp = DateTime.Now
                };

                // Find the sender
                saveSession.Receiver = this.MapNodeInfo(audit.Receiver);
                saveSession.Sender = this.MapNodeInfo(audit.SenderNode);

            }

            // Errors?
            if (audit.Errors != null)
            {
                foreach (var err in audit.Errors)
                {
                    saveSession.AuditErrors.Add(new AuditError()
                    {
                        ErrorMessage = err.Message,
                        StackTrace = err.StackTrace,
                        AuditMessageId = audit.CorrelationToken
                    });
                }
            }

            // Do message 
            AuditMessage am = audit.Event;
            if (am != null)
            {
                Audit auditSave = new Audit();
                auditSave.GlobalId = audit.CorrelationToken;
                saveSession.Audits.Add(auditSave);
                auditSave.ProcessName = audit.SenderProcess;

                // Core event identification attributes
                if (am.EventIdentification != null)
                {
                    auditSave.ActionCode = this.GetAuditCode("ActionType", GetWireCode(am.EventIdentification.ActionCode), null);
                    auditSave.OutcomeCode = this.GetAuditCode("OutcomeIndicator", this.GetWireCode(am.EventIdentification.EventOutcome), null);

                    auditSave.EventCode = this.GetAuditCode(am.EventIdentification.EventId);

                    foreach (var stat in audit.StatusHistory)
                    {
                        auditSave.AuditStatus.Add(new AuditStatus()
                        {
                            StatusCode = this.m_context.StatusCodes.Where(o => o.Name == stat.StatusCode.ToString().ToUpper()).First(),
                            IsAlert = stat.IsAlert,
                            CreationTimestamp = stat.EffectiveFrom,
                            ObsoletionTimestamp = stat.EffectiveTo == default(DateTime) ? null : (DateTime?)stat.EffectiveTo,
                            ModifiedBy = Environment.UserName
                        });
                    }

                    // Identification type
                    if (am.EventIdentification.EventType != null)
                        foreach (var ei in am.EventIdentification.EventType)
                            auditSave.AuditEventTypeAuditCodeAssocs.Add(new AuditEventTypeAuditCodeAssoc()
                            {
                                EventTypeCode = this.GetAuditCode(ei)
                            });
                    auditSave.EventTimestamp = am.EventIdentification.EventDateTime;
                    auditSave.CreationTimestamp = DateTime.Now;
                }

                // Now for the audit participants
                if (am.Actors != null)
                    foreach (var act in am.Actors)
                        auditSave.AuditParticipants.Add(this.MapAuditActor(act));

                if (am.AuditableObjects != null)
                    foreach (var ao in am.AuditableObjects)
                        auditSave.AuditObjects.Add(this.MapAuditObject(ao));

                if (am.SourceIdentification != null)
                    foreach (var si in am.SourceIdentification)
                    {
                        var assoc = this.MapAuditSource(si);
                        assoc.Audit = auditSave;
                        auditSave.AuditAuditSourceAssocs.Add(assoc);
                    }

            }
            
            // Add
            this.m_context.AuditSessions.InsertOnSubmit(saveSession);

            try
            {
                this.m_context.SubmitChanges();
            }
            catch(SqlException ex)
            {
                Trace.TraceError(ex.Message);
                foreach (var kv in ex.Data.Keys)
                    Trace.TraceError("\t(DTL): {0}={1}", kv, ex.Data[kv]);
                throw;
            }
            if (this.Persisted != null)
                this.Persisted(this, evt);

        }

        /// <summary>
        /// Maps audit source information 
        /// </summary>
        private AuditAuditSourceAssoc MapAuditSource(AtnaApi.Model.AuditSourceIdentificationType si)
        {
            // Do we have one on file?
            var existing = this.m_context.AuditSources.FirstOrDefault(s => s.AuditSourceName == si.AuditSourceID && s.EnterpriseSiteName == si.AuditEnterpriseSiteID);
            if (existing == null && String.IsNullOrEmpty(si.AuditEnterpriseSiteID))
                existing = this.m_context.AuditSources.FirstOrDefault(s => s.AuditSourceName == si.AuditSourceID);

            if (existing != null)
                return new AuditAuditSourceAssoc()
                {
                    AuditSource = existing
                };

            var retVal = new AuditSource();

            retVal.EnterpriseSiteName = si.AuditEnterpriseSiteID;
            retVal.AuditSourceName = si.AuditSourceID;

            if (si.AuditSourceTypeCode != null)
                foreach (var astc in si.AuditSourceTypeCode)
                    retVal.AuditSourceTypeAssocs.Add(new AuditSourceTypeAssoc()
                    {
                        AuditSource = retVal,
                        TypeCode = this.GetAuditCode(astc)
                    });
            return new AuditAuditSourceAssoc()
            {
                AuditSource = retVal
            };
        }

        /// <summary>
        /// Map an audit object
        /// </summary>
        private AuditObject MapAuditObject(AtnaApi.Model.AuditableObject ao)
        {
            var retVal = new AuditObject();

            // Codes
            retVal.IdTypeCode = this.GetAuditCode(ao.IDTypeCode);
            if (ao.LifecycleTypeSpecified)
                retVal.LifecycleCode = this.GetAuditCode("AuditableObjectLifecycle", this.GetWireCode(ao.LifecycleType), null);
            if (ao.RoleSpecified)
                retVal.RoleCode = this.GetAuditCode("AuditableObjectRole", this.GetWireCode(ao.Role), null);
            if (ao.TypeSpecified)
                retVal.TypeCode = this.GetAuditCode("AuditableObjectType", this.GetWireCode(ao.Type), null);
            retVal.ExternalIdentifier = ao.ObjectId;

            // Object specifics
            if (ao.ObjectSpec != null)
            {
                retVal.ObjectSpec = ao.ObjectSpec;
                retVal.ObjectSpecType = ao.ObjectSpecChoice == AtnaApi.Model.ObjectDataChoiceType.ParticipantObjectName ? 'N' : 'Q';

            }

            if (ao.ObjectDetail != null)
                foreach (var od in ao.ObjectDetail)
                    retVal.AuditObjectDetails.Add(new AuditObjectDetail()
                    {
                        DetailType = od.Type,
                        DetailValue = od.Value
                    });

            return retVal;

        }

        /// <summary>
        /// Map an audit actor
        /// </summary>
        private AuditParticipant MapAuditActor(AtnaApi.Model.AuditActorData act)
        {
            var retVal = new AuditParticipant();

            foreach (var rol in act.ActorRoleCode)
                retVal.AuditParticipantRoleCodeAssocs.Add(
                    new AuditParticipantRoleCodeAssoc()
                    {
                        AuditParticipant = retVal,
                        RoleCode = this.GetAuditCode(rol)
                    });



            retVal.NetworkAccessPoint = act.NetworkAccessPointId;
            retVal.IsRequestor = act.UserIsRequestor;

            // First is there a node with the user identifier? 
            retVal.NodeVersion = this.m_context.NodeVersions.FirstOrDefault(n => n.Name == act.UserIdentifier || n.HostName == act.NetworkAccessPointId);

            retVal.RawUserId = act.UserIdentifier;
            retVal.RawUserName = act.UserName;
            return retVal;
        }

        /// <summary>
        /// Get the audit code
        /// </summary>
        private AuditCode GetAuditCode<T>(AtnaApi.Model.CodeValue<T> codeValue)
        {
            return this.GetAuditCode(codeValue.CodeSystem, codeValue.Code, codeValue.DisplayName);
        }

        /// <summary>
        /// Get the audit code from the DB
        /// </summary>
        public AuditCode GetAuditCode(String domain, String code, String display)
        {

            if (String.IsNullOrEmpty(domain) ||
                String.IsNullOrEmpty(code))
            {
                Trace.TraceWarning("Code {0}^^^{1} - {2} is missing information", code, domain, display);
                domain = "";
            }

            int? codeIdParameter = 0;
            this.m_context.sp_CreateAuditCodeIfNotExists(domain, code, display, ref codeIdParameter);
            return this.m_context.AuditCodes.Where(o => o.CodeId == (Int32)codeIdParameter.Value).First();
        }

        /// <summary>
        /// Get the wire level code
        /// </summary>
        public String GetWireCode(Object codeEnum)
        {
            if (codeEnum == null || !codeEnum.GetType().IsEnum) return null;

            var fieldInfo = codeEnum.GetType().GetField(codeEnum.ToString());
            var enumValue = fieldInfo.GetCustomAttributes(typeof(XmlEnumAttribute), false);
            if (enumValue.Length == 0) return null;
            else return (enumValue[0] as XmlEnumAttribute).Name;

        }

        /// <summary>
        /// Map node info
        /// </summary>
        private NodeVersion MapNodeInfo(NodeInfo nodeInfo)
        {
            var retVal = this.m_context.NodeVersions.Where(o => o.HostName == nodeInfo.Host.Host && o.ObsoletionTime == null).OrderByDescending(o=>o.NodeVersionId).FirstOrDefault();

            if(retVal == null)
                retVal = new NodeVersion()
                {
                    Node = new Node(),
                    CreationTimestamp = DateTime.Now,
                    Name = nodeInfo.Name,
                    HostName = nodeInfo.Host.Host,
                    StatusCodeId = (int)nodeInfo.Status,
                    NodeMagic = nodeInfo.X509Thumbprint != null ? new System.Data.Linq.Binary(Convert.FromBase64String(nodeInfo.X509Thumbprint)) : null
                };
            return retVal;
        }

        /// <summary>
        /// Load a message back from the database
        /// </summary>
        public AuditMessageInfo ConvertAuditMessage(Audit audit)
        {
            // Reconstitute
            var retVal = new AuditMessageInfo()
            {
                Id = audit.AuditId,
                CreationTime = audit.CreationTimestamp,
                SenderProcess = audit.ProcessName,
                SessionId = audit.SessionId,
                CorrelationToken = audit.GlobalId,
                Event = new AuditMessage(),
                SenderNode = ParseNodeVersion(audit.AuditSession.Sender),
                Receiver = ParseNodeVersion(audit.AuditSession.Receiver)
            };


            // Audit stuffs
            retVal.Event.EventIdentification = new AtnaApi.Model.EventIdentificationType()
            {
                EventDateTime = audit.EventTimestamp,
                ActionCode = this.ParseCodeValue<AtnaApi.Model.ActionType>(audit.ActionCode).StrongCode,
                EventId = this.ParseCodeValue<AtnaApi.Model.EventIdentifierType>(audit.EventCode),
                EventOutcome = this.ParseCodeValue<AtnaApi.Model.OutcomeIndicator>(audit.OutcomeCode).StrongCode,
                EventType = new List<AtnaApi.Model.CodeValue<string>>()
            };
            retVal.Event.SourceIdentification = new List<AtnaApi.Model.AuditSourceIdentificationType>();
            retVal.Event.Actors = new List<AtnaApi.Model.AuditActorData>();
            retVal.Event.AuditableObjects = new List<AtnaApi.Model.AuditableObject>();

            // Event types
            foreach (var itm in audit.AuditEventTypeAuditCodeAssocs)
                retVal.Event.EventIdentification.EventType.Add(this.ParseCodeValue<String>(itm.EventTypeCode));

            // Source id
            foreach (var itm in audit.AuditAuditSourceAssocs)
            {
                var sid = new AtnaApi.Model.AuditSourceIdentificationType()
                {
                    AuditEnterpriseSiteID = itm.AuditSource.EnterpriseSiteName,
                    AuditSourceID = itm.AuditSource.AuditSourceName,
                    AuditSourceTypeCode = new List<AtnaApi.Model.CodeValue<AtnaApi.Model.AuditSourceType>>()
                };
                foreach (var tc in itm.AuditSource.AuditSourceTypeAssocs)
                    sid.AuditSourceTypeCode.Add(this.ParseCodeValue<AtnaApi.Model.AuditSourceType>(tc.TypeCode));
                retVal.Event.SourceIdentification.Add(sid);
            }

            // Reverse map actors
            foreach (var itm in audit.AuditParticipants)
            {
                var actor = new ExtendedActorInfo()
                {
                    ActorRoleCode = new List<AtnaApi.Model.CodeValue<string>>(),
                    NetworkAccessPointId = itm.NetworkAccessPoint,
                    NetworkAccessPointType = AtnaApi.Model.NetworkAccessPointType.Unknown,
                    NetworkAccessPointTypeSpecified = true,
                    UserIdentifier = itm.RawUserId,
                    Node = ParseNodeVersion(itm.NodeVersion),
                    UserIsRequestor = itm.IsRequestor,
                    UserName = itm.RawUserName
                };

                foreach (var rc in itm.AuditParticipantRoleCodeAssocs)
                    actor.ActorRoleCode.Add(this.ParseCodeValue<String>(rc.RoleCode));
                retVal.Event.Actors.Add(actor);
            }

            // Reverse map participants
            foreach (var itm in audit.AuditObjects)
            {
                var lifecycleCode = this.ParseCodeValue<AtnaApi.Model.AuditableObjectLifecycle>(itm.LifecycleCode);
                var roleCode = this.ParseCodeValue<AtnaApi.Model.AuditableObjectRole>(itm.RoleCode);
                var typeCode = this.ParseCodeValue<AtnaApi.Model.AuditableObjectType>(itm.TypeCode);

                var obj = new AtnaApi.Model.AuditableObject()
                {
                    IDTypeCode = this.ParseCodeValue<AtnaApi.Model.AuditableObjectIdType>(itm.IdTypeCode),
                    ObjectId = itm.ExternalIdentifier,
                    ObjectDetail = new List<AtnaApi.Model.ObjectDetailType>(),
                    ObjectSpec = itm.ObjectSpec,
                    ObjectSpecChoice = itm.ObjectSpecType == 'N' ? AtnaApi.Model.ObjectDataChoiceType.ParticipantObjectName : AtnaApi.Model.ObjectDataChoiceType.ParticipantObjectQuery,
                };

                // Set codes
                if (lifecycleCode != null)
                {
                    obj.LifecycleTypeSpecified = true;
                    obj.LifecycleType = lifecycleCode.StrongCode;
                }
                if (roleCode != null)
                {
                    obj.Role = roleCode.StrongCode;
                    obj.RoleSpecified = true;
                }
                if (typeCode != null)
                {
                    obj.Type = typeCode.StrongCode;
                    obj.TypeSpecified = true;
                }

                // Data 
                foreach (var dat in itm.AuditObjectDetails)
                {
                    obj.ObjectDetail.Add(new AtnaApi.Model.ObjectDetailType()
                    {
                        Type = dat.DetailType,
                        Value = dat.DetailValue.ToArray()
                    });
                }
                retVal.Event.AuditableObjects.Add(obj);
            }

            retVal.StatusHistory = new List<AuditStatusEntry>();
            // Load statuses
            foreach (var stat in audit.AuditStatus)
            {
                retVal.StatusHistory.Add(new AuditStatusEntry()
                {
                    EffectiveFrom = stat.CreationTimestamp,
                    EffectiveTo = stat.ObsoletionTimestamp.HasValue ? stat.ObsoletionTimestamp.Value : default(DateTime),
                    StatusCode = (StatusType)stat.StatusCodeId,
                    IsAlert = stat.IsAlert.HasValue ? stat.IsAlert.Value : false,
                    SetByUserId = stat.ModifiedBy
                });
            }

            return retVal;
        }

        /// <summary>
        /// Parse a node version
        /// </summary>
        internal static NodeInfo ParseNodeVersion(NodeVersion nodeVersion)
        {
            return new NodeInfo()
            {
                GroupNode = null,
                Host = new Uri(String.Format("atna://{0}", nodeVersion.HostName)),
                Name = nodeVersion.Name,
                Id = nodeVersion.NodeId,
                Status = (StatusType)nodeVersion.StatusCodeId,
                X509Thumbprint = nodeVersion.NodeMagic != null ? Convert.ToBase64String(nodeVersion.NodeMagic.ToArray()) : null
            };
        }


        /// <summary>
        /// Parse the code value
        /// </summary>
        private AtnaApi.Model.CodeValue<T> ParseCodeValue<T>(AuditCode itm)
        {
            if (itm == null) return null;

            return new AtnaApi.Model.CodeValue<T>()
            {
                Code = itm.Mnemonic,
                DisplayName = itm.DisplayName,
                CodeSystem = itm.Domain
            };
        }

        /// <summary>
        /// Retrieves an audit message
        /// </summary>
        public Visualization.Core.Model.AuditMessageInfo DePersistAuditMessage(Guid correlationToken)
        {
            var audit = this.m_context.Audits.FirstOrDefault(o => o.GlobalId == correlationToken);
            if (audit != null)
                return this.ConvertAuditMessage(audit);
            return null;
        }

        /// <summary>
        /// Searches for an audit message
        /// </summary>
        public IEnumerable<Visualization.Core.Model.AuditMessageInfo> SearchAuditMessage(Visualization.Core.Model.AuditMessageInfo prototype)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fired when the message is persisting
        /// </summary>
        public event EventHandler<Visualization.Core.Services.AuditPersistenceEventArgs> Persisting;

        /// <summary>
        /// Fired when the message has been persisted
        /// </summary>
        public event EventHandler<Visualization.Core.Services.AuditPersistenceEventArgs> Persisted;

        /// <summary>
        /// Fired when a message has been depersisted
        /// </summary>
        public event EventHandler<Visualization.Core.Services.AuditPersistenceEventArgs> DePersisted;

        #endregion

        #region IUsesHostContext Members

        /// <summary>
        /// When the service is "hooked up"
        /// </summary>
        public IServiceProvider Context
        {
            get
            {
                return null;
            }
            set
            {
                this.m_context = new AuditModelDataContext();
            }
        }

        #endregion
    }
}
