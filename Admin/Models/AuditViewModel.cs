using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using AtnaApi.Model;
using Admin.DataAccess;
using MARC.EHRS.Visualization.Core.Model;

namespace Admin.Models
{
    /// <summary>
    /// Audit view model based on an audit
    /// </summary>
    public class AuditViewModel
    {

        /// <summary>
        /// Audit view model ctor
        /// </summary>
        public AuditViewModel(int auditId, AuditSummaryVw dalModel, AuditMessageInfo audit)
        {
            
            this.Audit = audit;
            this.Id = auditId;
            this.DalModel = dalModel;
        }

        /// <summary>
        /// ID of the audit
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the actual audit
        /// </summary>
        public AuditMessageInfo Audit { get; set; }

        /// <summary>
        /// Get the source system(s)
        /// </summary>
        public ActorInfo SourceUser
        {
            get
            {
                var ret = this.Audit.Event.Actors.FirstOrDefault(a => a.UserIsRequestor && a.ActorRoleCode.Count(r => r.Code == "110153") == 0);
                if (ret is ActorInfo)
                    return ret as ActorInfo;
                else if (ret != null)
                    return new ActorInfo()
                    {
                        ActorRoleCode = ret.ActorRoleCode,
                        AlternativeUserId = ret.AlternativeUserId,
                        NetworkAccessPointId = ret.NetworkAccessPointId,
                        NetworkAccessPointType = ret.NetworkAccessPointType,
                        UserIdentifier = ret.UserIdentifier,
                        UserIsRequestor = ret.UserIsRequestor,
                        UserName = ret.UserName
                    };
                else
                    return null;

            }
        }
        
        /// <summary>
        /// Get the source users system(s)
        /// </summary>
        public AuditActorData SourceSystem
        {
            get
            {
                return this.Audit.Event.Actors.FirstOrDefault(a => a.ActorRoleCode.Count(r => r.Code == "110153") > 0);
                
            }
        }

        /// <summary>
        /// Get the source users system(s)
        /// </summary>
        public AuditActorData DestinationSystem
        {
            get
            {
                return this.Audit.Event.Actors.FirstOrDefault(a => a.ActorRoleCode.Count(r => r.Code == "110152") > 0);
            }
        }


        public AuditSummaryVw DalModel { get; set; }
    }
}