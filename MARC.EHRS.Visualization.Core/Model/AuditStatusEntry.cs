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
using System;

namespace MARC.EHRS.Visualization.Core.Model
{
	/// <summary>
	/// Represents a status entry
	/// </summary>
	public class AuditStatusEntry
	{
		/// <summary>
		/// Represents the effective time
		/// </summary>
		public DateTime EffectiveFrom { get; set; }

		/// <summary>
		/// Represents the obsoletion time
		/// </summary>
		public DateTime EffectiveTo { get; set; }

		/// <summary>
		/// True if the status condition is an alert
		/// </summary>
		public bool IsAlert { get; set; }

		/// <summary>
		/// Set by the user identifier
		/// </summary>
		public String SetByUserId { get; set; }

		/// <summary>
		/// Represetns the status code
		/// </summary>
		public StatusType StatusCode { get; set; }
	}
}