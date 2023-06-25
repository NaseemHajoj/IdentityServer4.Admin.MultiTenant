using System;
using System.Threading;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Validation;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity;

using static IdentityModel.OidcConstants;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    public class MyResourceOwnerPasswordValidator<TUserIdentity> : IResourceOwnerPasswordValidator
		where TUserIdentity : ApplicationUser<string>, new()
	{
		// Tenant token in the request.
		public const string TenantKey = "tenant";

		private readonly ApplicationUserManager<TUserIdentity> applicationUserManager;

		private readonly SignInManager<TUserIdentity> signInManager;

		private readonly ITenantStore tenantStore;

		private readonly ILogger<MyResourceOwnerPasswordValidator<TUserIdentity>> logger;

		public MyResourceOwnerPasswordValidator(
			ApplicationUserManager<TUserIdentity> userManager,
			SignInManager<TUserIdentity> signInManager,
			ITenantStore tenantStore,
			ILogger<MyResourceOwnerPasswordValidator<TUserIdentity>> logger)
		{
			this.applicationUserManager = userManager;
			this.signInManager = signInManager;
			this.tenantStore = tenantStore;
			this.logger = logger;
		}

		/// <inheritdoc />
		public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
		{
			try
			{
				CancellationToken cancellationToken = CancellationToken.None;

                Tenant tenant = await this.ValidateAndGetTenantAsync(context, cancellationToken);
				if (tenant == null)
				{
					// already logged and added proper response
					return;
				}
				else if (string.IsNullOrWhiteSpace(context.UserName) || string.IsNullOrWhiteSpace(context.Password))
				{
					context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, "Username or password are not provided");
					return;
				}

				await this.ValidateAsync(context, tenant.Name);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Failed to validate resource owner password");

				context.Result = new GrantValidationResult(
					TokenRequestErrors.InvalidRequest,
					$"An internal error occurred: {ex.Message}");
			}
		}

		/// <summary>
		/// Validate the tenant in request is a valid existing tenant in the store.
		/// The <paramref name="context"/> is updated with the grant validation result.
		/// </summary>
		/// <returns>true if tenant is valid, otherwise false.</returns>
		private async Task<Tenant> ValidateAndGetTenantAsync(ResourceOwnerPasswordValidationContext context, CancellationToken cancellationToken)
		{
			string tenantId = context.Request.Raw.Get(TenantKey);

			if (string.IsNullOrWhiteSpace(tenantId))
			{
				context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"Missing '{TenantKey}' parameter");
				return default;
			}

			if (!Guid.TryParse(tenantId, out Guid extractedTenantId))
			{
				context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"Invalid value for parameter '{TenantKey}': Expected a valid Guid");
				return default;
			}

            Tenant tenant = await this.tenantStore.GetTenantAsync(extractedTenantId, cancellationToken);
			if (tenant == null)
			{
				context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"Invalid Tenant");
				return default;
			}

			return tenant;
		}

		private async Task ValidateAsync(ResourceOwnerPasswordValidationContext context, string tenantName)
		{
			TUserIdentity user = await this.applicationUserManager.FindByNameAsync(context.UserName, tenantName);
			if (user == default(TUserIdentity))
			{
				this.logger.LogInformation("No user found matching username: {username}", context.UserName);
				return;
			}

			SignInResult result = await this.signInManager.CheckPasswordSignInAsync(user, context.Password, true);
			if (result.Succeeded)
			{
				var sub = await this.applicationUserManager.GetUserIdAsync(user);

				this.logger.LogInformation("Credentials validated for username: {username}", context.UserName);

				context.Result = new GrantValidationResult(sub, AuthenticationMethods.Password);
				return;
			}
			else if (result.IsLockedOut)
			{
				this.logger.LogInformation("Authentication failed for username: {username}, reason: locked out", context.UserName);
			}
			else if (result.IsNotAllowed)
			{
				this.logger.LogInformation("Authentication failed for username: {username}, reason: not allowed", context.UserName);
			}
			else
			{
				this.logger.LogInformation("Authentication failed for username: {username}, reason: invalid credentials", context.UserName);
			}

			context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
		}
	}
}
