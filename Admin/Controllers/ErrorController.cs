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
 * Date: 2017-6-16
 */

using System;
using System.Web.Mvc;

namespace Admin.Controllers
{
	/// <summary>
	/// Represents an error controller.
	/// </summary>
	/// <seealso cref="System.Web.Mvc.Controller" />
	public class ErrorController : Controller
	{
		/// <summary>
		/// Displays the forbidden view.
		/// </summary>
		/// <param name="aspxerrorpath">The aspxerrorpath.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance/</returns>
		public ActionResult Forbidden(String aspxerrorpath)
		{
			return View();
		}

		/// <summary>
		/// Displays the not found view.
		/// </summary>
		/// <param name="aspxerrorpath">The aspxerrorpath.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance/</returns>
		public ActionResult NotFound(String aspxerrorpath)
		{
			return View();
		}

		/// <summary>
		/// Displays the unauthorized view.
		/// </summary>
		/// <param name="aspxerrorpath">The aspxerrorpath.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance/</returns>
		public ActionResult Unauthorized(String aspxerrorpath)
		{
			return View();
		}

		/// <summary>
		/// Displays the unavailable view.
		/// </summary>
		/// <param name="aspxerrorpath">The aspxerrorpath.</param>
		/// <returns>Returns an <see cref="ActionResult"/> instance/</returns>
		public ActionResult Unavailable(String aspxerrorpath)
		{
			return View();
		}
	}
}