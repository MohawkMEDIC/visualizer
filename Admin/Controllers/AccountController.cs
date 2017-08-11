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

using Admin.Attributes;
using Admin.Models;
using Admin.Util;
using AtnaApi.Model;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
	/// <summary>
	/// Represents an account controller.
	/// </summary>
	/// <seealso cref="System.Web.Mvc.Controller" />
	[TokenAuthorize]
	public class AccountController : Controller
	{
		/// <summary>
		/// The sign in manager.
		/// </summary>
		private ApplicationSignInManager _signInManager;

		/// <summary>
		/// The user manager.
		/// </summary>
		private ApplicationUserManager _userManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController"/> class.
		/// </summary>
		public AccountController()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AccountController"/> class.
		/// </summary>
		/// <param name="userManager">The user manager.</param>
		/// <param name="signInManager">The sign in manager.</param>
		public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
		{
			UserManager = userManager;
			SignInManager = signInManager;
		}

		/// <summary>
		/// Gets the sign in manager.
		/// </summary>
		/// <value>The sign in manager.</value>
		public ApplicationSignInManager SignInManager
		{
			get
			{
				return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
			}
			private set
			{
				_signInManager = value;
			}
		}

		/// <summary>
		/// Gets the user manager.
		/// </summary>
		/// <value>The user manager.</value>
		public ApplicationUserManager UserManager
		{
			get
			{
				return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
			}
			private set
			{
				_userManager = value;
			}
		}

		//
		// GET: /Account/Login
		[AllowAnonymous]
		public ActionResult Login(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}

		//
		// POST: /Account/Login
		[HttpPost]
		[AllowAnonymous]
		[ValidateAntiForgeryToken]
		public async Task<ActionResult> Login(LoginModel model, string returnUrl)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			OutcomeIndicator outcome = OutcomeIndicator.Success;
			try
			{
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, change to shouldLockout: true
				var result = await SignInManager.PasswordSignInAsync(model.Username, model.Password, model.RememberMe, shouldLockout: false);
				switch (result)
				{
					case SignInStatus.Success:
						Response.Cookies.Add(new HttpCookie("access_token", SignInManager.AccessToken));
						return RedirectToLocal(returnUrl);

					case SignInStatus.LockedOut:
						outcome = OutcomeIndicator.EpicFail;
						return View("Lockout");

					case SignInStatus.Failure:
					default:
						outcome = OutcomeIndicator.SeriousFail;
						ModelState.AddModelError("", "Invalid login attempt.");
						return View(model);
				}
			}
			finally
			{
				AuditUtil.AuditUserEvent(this, outcome, ActionType.Execute, EventIdentifierType.Login, AuditableObjectLifecycle.Access, null);
			}
		}

		//
		// POST: /Account/LogOff
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult LogOff()
		{
			// sign out the user and remove the access token cookie from the response
			AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
			Response.Cookies.Remove("access_token");

			AuditUtil.AuditUserEvent(this, OutcomeIndicator.Success, ActionType.Execute, EventIdentifierType.Logout, AuditableObjectLifecycle.Access, null);
			return RedirectToAction("Index", "Home");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_userManager != null)
				{
					_userManager.Dispose();
					_userManager = null;
				}

				if (_signInManager != null)
				{
					_signInManager.Dispose();
					_signInManager = null;
				}
			}

			base.Dispose(disposing);
		}

		#region Helpers

		// Used for XSRF protection when adding external logins
		private const string XsrfKey = "XsrfId";

		private IAuthenticationManager AuthenticationManager
		{
			get
			{
				return HttpContext.GetOwinContext().Authentication;
			}
		}

		private void AddErrors(IdentityResult result)
		{
			foreach (var error in result.Errors)
			{
				ModelState.AddModelError("", error);
			}
		}

		private ActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			return RedirectToAction("Index", "Home");
		}

		internal class ChallengeResult : HttpUnauthorizedResult
		{
			public ChallengeResult(string provider, string redirectUri)
				: this(provider, redirectUri, null)
			{
			}

			public ChallengeResult(string provider, string redirectUri, string userId)
			{
				LoginProvider = provider;
				RedirectUri = redirectUri;
				UserId = userId;
			}

			public string LoginProvider { get; set; }
			public string RedirectUri { get; set; }
			public string UserId { get; set; }

			public override void ExecuteResult(ControllerContext context)
			{
				var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
				if (UserId != null)
				{
					properties.Dictionary[XsrfKey] = UserId;
				}
				context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
			}
		}

        #endregion Helpers

        protected override void OnException(ExceptionContext filterContext)
        {
            Trace.TraceError("Error on controller: {0}", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}