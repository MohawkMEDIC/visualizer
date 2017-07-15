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
using MARC.Everest.Connectors;
using System;

namespace MARC.EHRS.Visualization.Core.Model
{
	/// <summary>
	/// Represents an audit error information
	/// </summary>
	public class AuditErrorInfo : StoredData
	{
		/// <summary>
		/// Default ctor
		/// </summary>
		public AuditErrorInfo()
		{
		}

		/// <summary>
		/// Create an audit error info from exception
		/// </summary>
		public AuditErrorInfo(Exception e)
		{
			this.Message = e.Message;
			this.StackTrace = e.StackTrace;
			if (e.InnerException != null)
				this.CausedBy = new AuditErrorInfo(e.InnerException);
		}

		/// <summary>
		/// Create audit error info from result detail
		/// </summary>
		public AuditErrorInfo(IResultDetail dtl)
		{
			this.Message = dtl.Message;
			this.StackTrace = dtl.Location;
			if (dtl.Exception != null)
				this.CausedBy = new AuditErrorInfo(dtl.Exception);
		}

		/// <summary>
		/// Gets the error that caused this
		/// </summary>
		public AuditErrorInfo CausedBy { get; set; }

		/// <summary>
		/// Gets or sets the textual message
		/// </summary>
		public String Message { get; set; }

		/// <summary>
		/// Gets or sets the stack trace
		/// </summary>
		public string StackTrace { get; set; }
	}
}