﻿/*
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
	/// Represents an class that can persist and retrieve NodeInfo and ProcessInfo structures
	/// </summary>
	public interface INodeInfoLookupService : IUsesHostContext
	{
		/// <summary>
		/// Get node information
		/// </summary>
		NodeInfo GetNodeInfo(Uri nodeEndpoint, bool includeHistory);

		/// <summary>
		/// Get node information by id
		/// </summary>
		NodeInfo GetNodeInfo(decimal nodeId, bool includeHistory);

		/// <summary>
		/// Search for node information
		/// </summary>
		List<NodeInfo> SearchNodeInfo(NodeInfo prototype);
	}
}