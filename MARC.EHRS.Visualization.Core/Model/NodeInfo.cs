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
	/// Represents a node on the network
	/// </summary>
	public class NodeInfo : StoredData
	{
		/// <summary>
		/// The node which is the grouping node for this node entry
		/// </summary>
		public NodeInfo GroupNode { get; set; }

		/// <summary>
		/// Gets or sets the configured host
		/// </summary>
		public Uri Host { get; set; }

		/// <summary>
		/// Gets or sets the name of the node
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The status of the node
		/// </summary>
		public StatusType Status { get; set; }

		/// <summary>
		/// Gets or sets the X509 certificate thumbprint
		/// </summary>
		public string X509Thumbprint { get; set; }
	}
}