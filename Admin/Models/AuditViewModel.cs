/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: khannan
 * Date: 2017-6-15
 */

using Admin.DataAccess;
using AtnaApi.Model;
using MARC.EHRS.Visualization.Core.Model;
using System.Linq;

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
		/// Gets or sets the actual audit
		/// </summary>
		public AuditMessageInfo Audit { get; set; }

		public AuditSummaryVw DalModel { get; set; }

		/// <summary>
		/// Get the source users system(s)
		/// </summary>
		public AuditActorData DestinationSystem
		{
			get
			{
				return this.Audit.Event.Actors.FirstOrDefault(a => !a.UserIsRequestor && a.ActorRoleCode.Count(r => r.Code == "110152") > 0);
			}
		}

		/// <summary>
		/// ID of the audit
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Get the source users system(s)
		/// </summary>
		public AuditActorData SourceSystem
		{
			get
			{
				return this.Audit.Event.Actors.FirstOrDefault(a => a.UserIsRequestor && a.ActorRoleCode.Count(r => r.Code == "110153") > 0);
			}
		}

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
                return this.Audit.Event.Actors.FirstOrDefault(a => a.UserIsRequestor && a.ActorRoleCode.Count(r => r.Code == "110153") > 0);
                
            }
        }

        /// <summary>
        /// Get the source users system(s)
        /// </summary>
        public AuditActorData DestinationSystem
        {
            get
            {
                return this.Audit.Event.Actors.FirstOrDefault(a => !a.UserIsRequestor && a.ActorRoleCode.Count(r => r.Code == "110152") > 0);
            }
        }


        public AuditSummaryVw DalModel { get; set; }
    }
}