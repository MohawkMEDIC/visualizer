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

using Admin.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Admin
{
	public class EmailService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your email service here to send an email.
			return Task.FromResult(0);
		}
	}

	public class SmsService : IIdentityMessageService
	{
		public Task SendAsync(IdentityMessage message)
		{
			// Plug in your SMS service here to send a text message.
			return Task.FromResult(0);
		}
	}

	// Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.
	public class ApplicationUserManager : UserManager<ApplicationUser>
	{
		public ApplicationUserManager(IUserStore<ApplicationUser> store)
			: base(store)
		{
		}

		public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
		{
			var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
			// Configure validation logic for usernames
			manager.UserValidator = new UserValidator<ApplicationUser>(manager)
			{
				AllowOnlyAlphanumericUserNames = false,
				RequireUniqueEmail = true
			};

			// Configure validation logic for passwords
			manager.PasswordValidator = new PasswordValidator
			{
				RequiredLength = 6,
				RequireNonLetterOrDigit = true,
				RequireDigit = true,
				RequireLowercase = true,
				RequireUppercase = true,
			};

			// Configure user lockout defaults
			manager.UserLockoutEnabledByDefault = true;
			manager.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(5);
			manager.MaxFailedAccessAttemptsBeforeLockout = 5;

			// Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
			// You can write your own provider and plug it in here.
			manager.RegisterTwoFactorProvider("Phone Code", new PhoneNumberTokenProvider<ApplicationUser>
			{
				MessageFormat = "Your security code is {0}"
			});
			manager.RegisterTwoFactorProvider("Email Code", new EmailTokenProvider<ApplicationUser>
			{
				Subject = "Security Code",
				BodyFormat = "Your security code is {0}"
			});
			manager.EmailService = new EmailService();
			manager.SmsService = new SmsService();
			var dataProtectionProvider = options.DataProtectionProvider;
			if (dataProtectionProvider != null)
			{
				manager.UserTokenProvider =
					new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
			}
			return manager;
		}
	}

	/// <summary>
	/// Represents the application sign in manager for the application.
	/// </summary>
	public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ApplicationSignInManager"/> class.
		/// </summary>
		/// <param name="userManager">The user manager.</param>
		/// <param name="authenticationManager">The authentication manager.</param>
		public ApplicationSignInManager(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
			: base(userManager, authenticationManager)
		{
		}

		/// <summary>
		/// Gets the access token.
		/// </summary>
		public string AccessToken { get; private set; }

		/// <summary>
		/// Called to generate the ClaimsIdentity for the user, override to add additional claims before SignIn
		/// </summary>
		/// <param name="user">The user.</param>
		/// <returns>Task&lt;ClaimsIdentity&gt;.</returns>
		public override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
		{
			return user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);
		}

		/// <summary>
		/// Creates the specified options.
		/// </summary>
		/// <param name="options">The options.</param>
		/// <param name="context">The context.</param>
		/// <returns>ApplicationSignInManager.</returns>
		public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
		{
			return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Authentication);
		}

		/// <summary>
		/// password sign in as an asynchronous operation.
		/// </summary>
		/// <param name="userName">Name of the user.</param>
		/// <param name="password">The password.</param>
		/// <param name="isPersistent">if set to <c>true</c> [is persistent].</param>
		/// <param name="shouldLockout">if set to <c>true</c> [should lockout].</param>
		/// <returns>Returns the sign in status of the process.</returns>
		/// <exception cref="InvalidOperationException">Domain to connect to was not found. Is there an &lt;add key='userDomain' value='' &gt; setup in the &lt;appSettings&gt; section?</exception>
		public override async Task<SignInStatus> PasswordSignInAsync(string userName, string password, bool isPersistent, bool shouldLockout)
		{
			var address = ConfigurationManager.AppSettings["userDomain"];

			if (address == null)
			{
				throw new InvalidOperationException("Domain to connect to was not found. Is there an <add key='userDomain' value='' /> setup in the <appSettings> section?");
			}

			var applicationId = ConfigurationManager.AppSettings["userDomainApplicationId"];
			var applicationSecret = ConfigurationManager.AppSettings["userDomainApplicationSecret"];

			using (var client = new HttpClient())
			{
				client.DefaultRequestHeaders.Add("Authorization", "BASIC " + Convert.ToBase64String(Encoding.UTF8.GetBytes(applicationId + ":" + applicationSecret)));

				var content = new StringContent($"grant_type=password&username={userName}&password={password}&scope={address}/imsi");

				// HACK: have to remove the headers before adding them...
				content.Headers.Remove("Content-Type");
				content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

				var result = await client.PostAsync($"{address}/auth/oauth2_token", content);

				if (result.IsSuccessStatusCode)
				{
					return await this.SignInAsync(result, userName);
				}

				return SignInStatus.Failure;
			}
		}

		/// <summary>
		/// sign in as an asynchronous operation.
		/// </summary>
		/// <param name="result">The result.</param>
		/// <param name="username">The username.</param>
		/// <returns>Task&lt;SignInStatus&gt;.</returns>
		private async Task<SignInStatus> SignInAsync(HttpResponseMessage result, string username)
		{
			var responseAsString = await result.Content.ReadAsStringAsync();

			var response = JObject.Parse(responseAsString);

			var accessToken = response.GetValue("access_token").ToString();
			var expiresIn = response.GetValue("expires_in").ToString();
			var tokenType = response.GetValue("token_type").ToString();
#if DEBUG
			Trace.TraceInformation("Access token: {0}", accessToken);
			Trace.TraceInformation("Expires in: {0}", expiresIn);
			Trace.TraceInformation("Token type {0}", tokenType);
#endif
			var authenticationDictionary = new Dictionary<string, string>
			{
				{"username", username},
				{ "access_token", accessToken},
				{ "token_type", tokenType}
			};

			var properties = new AuthenticationProperties(authenticationDictionary)
			{
				IsPersistent = false
			};

			var securityToken = new JwtSecurityToken(accessToken);

			var user = await this.UserManager.FindByNameAsync(securityToken.Claims.First(c => c.Type == "unique_name").Value);

			if (user == null)
			{
				user = this.CreateUser(securityToken);

				var identityResult = await this.UserManager.CreateAsync(user);

				if (!identityResult.Succeeded)
				{
					return SignInStatus.Failure;
				}

				var userIdentity = await this.CreateUserIdentityAsync(user);

				this.AuthenticationManager.SignIn(properties, userIdentity);
				this.AccessToken = accessToken;
			}
			else
			{
				// delete the user, to avoid conflicts
				// this is safe, because we don't have any user created tables which depend on the users table
				await this.UserManager.DeleteAsync(user);

				user = this.CreateUser(securityToken);

				var identityResult = await this.UserManager.CreateAsync(user);

				if (!identityResult.Succeeded)
				{
					return SignInStatus.Failure;
				}

				var userIdentity = await this.CreateUserIdentityAsync(user);

				this.AuthenticationManager.SignIn(properties, userIdentity);
				this.AccessToken = accessToken;
			}

			return SignInStatus.Success;
		}

		/// <summary>
		/// Creates the user.
		/// </summary>
		/// <param name="securityToken">The security token.</param>
		/// <returns>ApplicationUser.</returns>
		private ApplicationUser CreateUser(JwtSecurityToken securityToken)
		{
			var user = new ApplicationUser
			{
				Id = securityToken.Claims.First(c => c.Type == "sub").Value,
				UserName = securityToken.Claims.First(c => c.Type == "unique_name").Value
			};

			string email = securityToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

			if (email != null)
			{
				if (email.StartsWith("mailto:"))
				{
					email = email.Substring(7, email.Length - 7);
				}

				user.Email = email;
			}

			foreach (var claim in securityToken.Claims)
			{
				var identityUserClaim = new IdentityUserClaim
				{
					ClaimType = claim.Type,
					ClaimValue = claim.Value,
					UserId = securityToken.Claims.First(c => c.Type == "sub").Value
				};

				user.Claims.Add(identityUserClaim);
			}

			return user;
		}
	}
}