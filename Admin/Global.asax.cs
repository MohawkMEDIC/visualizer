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

using Admin.Util;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Admin
{
	public class MvcApplication : System.Web.HttpApplication
	{
		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			RouteConfig.RegisterRoutes(RouteTable.Routes);
			BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		/// <summary>
		/// Application has rendered page, we want to audit if anything funky went on
		/// </summary>
		private void Application_EndRequest(object sender, System.EventArgs e)
		{
			string aspxerrorpath = Request.RawUrl;

			// If the user is not authorised to see this page or access this function, send them to the error page.
			switch (Response.StatusCode)
			{
				case 401:
					if (Request.Headers["Authorization"] != null)
					{ // failed password attempt
						AuditUtil.AuditGenericError(this.Context, AtnaApi.Model.OutcomeIndicator.EpicFail, AtnaApi.Model.EventIdentifierType.ApplicationActivity, AuditUtil.MakeGenericCode(AtnaApi.Model.EventIdentifierType.UserAuthentication), aspxerrorpath);
						Response.RedirectToRoute("Default", new { controller = "Error", action = "Unauthorized", aspxerrorpath = Request.RawUrl });
					}
					break;

				case 403:
					Response.ClearContent();
					AuditUtil.AuditGenericError(this.Context, AtnaApi.Model.OutcomeIndicator.EpicFail, AtnaApi.Model.EventIdentifierType.ApplicationActivity, AuditUtil.MakeGenericCode(AtnaApi.Model.EventIdentifierType.UseOfRestrictedFunction), aspxerrorpath);
					Response.RedirectToRoute("Default", new { controller = "Error", action = "Forbidden", aspxerrorpath = Request.RawUrl });
					break;

				case 503:
					Response.ClearContent();
					Response.RedirectToRoute("Default", new { controller = "Error", action = "Unavailable", aspxerrorpath = Request.RawUrl });
					break;
			}
		}
	}
}