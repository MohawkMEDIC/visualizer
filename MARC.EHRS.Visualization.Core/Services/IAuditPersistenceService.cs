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
using MARC.EHRS.Visualization.Core.Model;
using MARC.HI.EHRS.SVC.Core.Services;
using System;
using System.Collections.Generic;

namespace MARC.EHRS.Visualization.Core.Services
{
	/// <summary>
	/// Represents a service that can persist and de-persist audit messages
	/// </summary>
	public interface IAuditPersistenceService : IUsesHostContext
	{
		/// <summary>
		/// Fired when a record is de-persisted
		/// </summary>
		event EventHandler<AuditPersistenceEventArgs> DePersisted;

		/// <summary>
		/// Fired after persisting
		/// </summary>
		event EventHandler<AuditPersistenceEventArgs> Persisted;

		/// <summary>
		/// Fired prior to persisting of event
		/// </summary>
		event EventHandler<AuditPersistenceEventArgs> Persisting;

		/// <summary>
		/// De-persists an audit message
		/// </summary>
		AuditMessageInfo DePersistAuditMessage(Guid correlationToken);

		/// <summary>
		/// Persists the audit message
		/// </summary>
		void PersistAuditMessage(AuditMessageInfo audit);

		/// <summary>
		/// Search audit messages for those matching the prototype
		/// </summary>
		IEnumerable<AuditMessageInfo> SearchAuditMessage(AuditMessageInfo prototype);
	}

	/// <summary>
	/// Event arguments for persistence events
	/// </summary>
	public class AuditPersistenceEventArgs : EventArgs
	{
		/// <summary>
		/// Creates a new audit persistence event
		/// </summary>
		public AuditPersistenceEventArgs(AuditMessageInfo data)
		{
			this.Data = data;
		}

		/// <summary>
		/// When set to true signals a cancel
		/// </summary>
		public bool Cancel { get; set; }

		/// <summary>
		/// Gets the data related to the persistence
		/// </summary>
		public AuditMessageInfo Data { get; private set; }
	}
}