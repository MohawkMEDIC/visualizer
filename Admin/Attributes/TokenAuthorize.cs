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

using Microsoft.AspNet.Identity;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Admin.Attributes
{
	/// <summary>
	/// Validates against whether a user accessing a resource has the correct permissions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
	public class TokenAuthorize : AuthorizeAttribute
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TokenAuthorize"/> class.
		/// </summary>
		public TokenAuthorize()
		{
		}

		/// <summary>
		/// Determines whether the current <see cref="System.Security.Principal.IPrincipal"/> is authorized to access the requested resources.
		/// </summary>
		/// <param name="httpContext">The HTTP context.</param>
		/// <returns>Returns true if the current user is authorized to access the request resources.</returns>
		protected override bool AuthorizeCore(HttpContextBase httpContext)
		{
			var isAuthorized = false;

			var accessToken = httpContext.Request.Cookies["access_token"]?.Value;

			if (!string.IsNullOrEmpty(accessToken) && !string.IsNullOrWhiteSpace(accessToken))
			{
				try
				{
					isAuthorized = IsExpired(accessToken);
				}
				catch (Exception e)
				{
					isAuthorized = false;
					Trace.TraceError($"Unable to decode token: {e}");
				}
			}

			return base.AuthorizeCore(httpContext) && isAuthorized;
		}

		/// <summary>
		/// Handles an unauthorized request.
		/// </summary>
		/// <param name="filterContext">The filter context of the request.</param>
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{
			filterContext.HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

			filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
			{
				{ "action", "Login" },
				{ "controller", "Account" }
			});

			if (filterContext.HttpContext.Request.IsAjaxRequest())
			{
				filterContext.Result = new HttpUnauthorizedResult();

				filterContext.HttpContext.Response.Clear();
				filterContext.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
				filterContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;
			}

			filterContext.HttpContext.Response.Cookies.Remove("access_token");
		}

		/// <summary>
		/// Determines whether a JWT token is expired.
		/// </summary>
		/// <param name="token">The JWT token.</param>
		/// <returns>Returns true if the token is expired.</returns>
		/// <exception cref="System.ArgumentException">If the token is in an invalid format.</exception>
		private static bool IsExpired(string token)
		{
			JwtSecurityToken securityToken;

			if (!TryParse(token, out securityToken))
			{
				throw new ArgumentException($"Unable to parse token: { token }");
			}

			// is the token expired?
			return securityToken.ValidTo > DateTime.UtcNow;
		}

		/// <summary>
		/// Attempts to parse a JWT token.
		/// </summary>
		/// <param name="token">The JWT token to parse.</param>
		/// <param name="parsedToken">The parsed token.</param>
		/// <returns>Returns true if the token is parsed successfully.</returns>
		private static bool TryParse(string token, out JwtSecurityToken parsedToken)
		{
			bool status;

			try
			{
				parsedToken = new JwtSecurityToken(token);
				status = true;
			}
			catch
			{
				// unable to parse token.
				status = false;
				parsedToken = null;
			}

			return status;
		}
	}
}