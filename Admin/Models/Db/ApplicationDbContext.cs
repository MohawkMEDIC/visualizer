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

using Microsoft.AspNet.Identity.EntityFramework;

namespace Admin.Models.Db
{
	/// <summary>
	/// Represents the application db context.
	/// </summary>
	/// <seealso cref="Microsoft.AspNet.Identity.EntityFramework.IdentityDbContext{ApplicationUser}" />
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationDbContext" /> class.
		/// </summary>
		public ApplicationDbContext()
			: base("VisualizerConnection", throwIfV1Schema: false)
		{
		}

		/// <summary>
		/// Creates this instance.
		/// </summary>
		/// <returns>ApplicationDbContext.</returns>
		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
	}
}