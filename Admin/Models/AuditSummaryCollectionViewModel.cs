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
using System.Collections.Generic;
using System.Linq;

namespace Admin.Models
{
	/// <summary>
	/// Represents an audit summary collection view model.
	/// </summary>
	public class AuditSummaryCollectionViewModel
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AuditSummaryCollectionViewModel"/> class.
		/// </summary>
		public AuditSummaryCollectionViewModel()
		{
			this.ActionCodes = new List<string>();
			this.AuditSourceNames = new List<string>();
			this.AuditSourceTypes = new List<string>();
			this.EnterpriseSiteNames = new List<string>();
			this.EventCodes = new List<string>();
			this.EventTypes = new List<string>();
			this.ObjectIdTypeCodes = new List<string>();
			this.ObjectLifecycleCodes = new List<string>();
			this.ObjectRoleCodes = new List<string>();
			this.ObjectTypeCodes = new List<string>();
			this.OutcomeCodes = new List<string>();
			this.ParticipationRoleCodes = new List<string>();
			this.UserIds = new List<string>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AuditSummaryCollectionViewModel"/> class.
		/// </summary>
		/// <param name="results">The results.</param>
		public AuditSummaryCollectionViewModel(IQueryable<AuditSummaryVw> results) : this()
		{
			this.Results = results;
		}

		/// <summary>
		/// Gets or sets the action codes.
		/// </summary>
		/// <value>The action codes.</value>
		public List<string> ActionCodes { get; set; }

		/// <summary>
		/// Gets or sets the audit source types.
		/// </summary>
		/// <value>The audit source types.</value>
		public List<string> AuditSourceTypes { get; set; }

		/// <summary>
		/// Gets or sets the audit source names.
		/// </summary>
		/// <value>The audit source names.</value>
		public List<string> AuditSourceNames { get; set; }

		/// <summary>
		/// Gets or sets the enterprise site names.
		/// </summary>
		/// <value>The enterprise site names.</value>
		public List<string> EnterpriseSiteNames { get; set; }

		/// <summary>
		/// Gets or sets the event codes.
		/// </summary>
		/// <value>The event codes.</value>
		public List<string> EventCodes { get; set; }

		/// <summary>
		/// Gets or sets the event types.
		/// </summary>
		/// <value>The event types.</value>
		public List<string> EventTypes { get; set; }

		/// <summary>
		/// Gets or sets the object identifier type codes.
		/// </summary>
		/// <value>The object identifier type codes.</value>
		public List<string> ObjectIdTypeCodes { get; set; }

		/// <summary>
		/// Gets or sets the object lifecycle codes.
		/// </summary>
		/// <value>The object lifecycle codes.</value>
		public List<string> ObjectLifecycleCodes { get; set; }

		/// <summary>
		/// Gets or sets the object role codes.
		/// </summary>
		/// <value>The object role codes.</value>
		public List<string> ObjectRoleCodes { get; set; }

		/// <summary>
		/// Gets or sets the object type codes.
		/// </summary>
		/// <value>The object type codes.</value>
		public List<string> ObjectTypeCodes { get; set; }

		/// <summary>
		/// Gets or sets the outcome codes.
		/// </summary>
		/// <value>The outcome codes.</value>
		public List<string> OutcomeCodes { get; set; }

		/// <summary>
		/// Gets or sets the participation role codes.
		/// </summary>
		/// <value>The participation role codes.</value>
		public List<string> ParticipationRoleCodes { get; set; }

		/// <summary>
		/// Gets or sets the results.
		/// </summary>
		/// <value>The results.</value>
		public IQueryable<AuditSummaryVw> Results { get; set; }

		/// <summary>
		/// Gets or sets the user ids.
		/// </summary>
		/// <value>The user ids.</value>
		public List<string> UserIds { get; set; }
	}
}