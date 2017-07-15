/*
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
using AtnaApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MARC.EHRS.Visualization.Core.Model
{
	/// <summary>
	/// Audit event information
	/// </summary>
	public class AuditMessageInfo : StoredData
	{
		/// <summary>
		/// Status history
		/// </summary>
		public AuditMessageInfo()
		{
			this.StatusHistory = new List<AuditStatusEntry>();
			this.Errors = new List<AuditErrorInfo>();
		}

		/// <summary>
		/// Gets or sets the correlation token
		/// </summary>
		public Guid CorrelationToken { get; set; }

		/// <summary>
		/// The time the audit message was created
		/// </summary>
		public DateTime CreationTime { get; set; }

		/// <summary>
		/// Gets or sets the audit error information
		/// </summary>
		public List<AuditErrorInfo> Errors { get; set; }

		/// <summary>
		/// The processed audit event
		/// </summary>
		public AuditMessage Event { get; set; }

		/// <summary>
		/// Gets or sets the receiver node
		/// </summary>
		public NodeInfo Receiver { get; set; }

		/// <summary>
		/// Gets or sets the sending node
		/// </summary>
		public NodeInfo SenderNode { get; set; }

		/// <summary>
		/// Gets or sets the sender process
		/// </summary>
		public String SenderProcess { get; set; }

		/// <summary>
		/// The session identifier
		/// </summary>
		public Guid SessionId { get; set; }

		/// <summary>
		/// Gets the current status
		/// </summary>
		public AuditStatusEntry Status { get { return this.StatusHistory.Last(o => o.EffectiveTo == default(DateTime)); } }

		/// <summary>
		/// Gets the status history
		/// </summary>
		public List<AuditStatusEntry> StatusHistory { get; set; }
	}
}