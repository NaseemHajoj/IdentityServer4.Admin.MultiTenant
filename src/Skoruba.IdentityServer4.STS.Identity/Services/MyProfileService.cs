using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityModel;

using IdentityServer4.AspNetIdentity;
using IdentityServer4.Models;
using IdentityServer4.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity;

using Claims = Skoruba.IdentityServer4.STS.Identity.Constants.Claims;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
	public class MyProfileService<TUserIdentity> : global::IdentityServer4.AspNetIdentity.ProfileService<TUserIdentity>
        where TUserIdentity : ApplicationUser<string>, new()
	{
		private readonly ApplicationUserManager<TUserIdentity> applicationUserManager;

		private readonly ILogger<MyProfileService<TUserIdentity>> logger;

		public MyProfileService(
			ApplicationUserManager<TUserIdentity> applicationUserManager,
			IUserClaimsPrincipalFactory<TUserIdentity> userClaimsFactory,
			ILogger<MyProfileService<TUserIdentity>> logger)
			: base(applicationUserManager, userClaimsFactory)
		{
			this.applicationUserManager = applicationUserManager;
			this.logger = logger;
		}

		/// <inheritdoc />
		public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
		{
			try
			{
				await base.GetProfileDataAsync(context);

				TUserIdentity user = await this.GetUserAsync(context.Subject);

				this.logger.LogDebug("Get the user claims and add them to the context.");

				// Get the roles of the user
				this.logger.LogTrace("Get user roles");
				IList<string> userRoles = await this.applicationUserManager.GetRolesAsync(user);
				this.logger.LogTrace($"Retrieved {userRoles.Count} user roles for user {user.Id}");

				IEnumerable<Claim> claims = await this.GetUserClaims(user, userRoles);

				// this will filter out the claims given context.RequestedClaimTypes which are defined
				// in IdentityServerConfigs and add the filtered claims to the context.
				context.IssuedClaims.AddRange(claims);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, $"Could not create user claims in {nameof(MyProfileService<TUserIdentity>)}.");
			}
		}

		/// <summary>
		/// Get the user curried with <paramref name="claimsPrincipal"/> from the store.
		/// </summary>
		private async Task<TUserIdentity> GetUserAsync(ClaimsPrincipal claimsPrincipal)
		{
			// Get subject (user Id) from context (this was set ResourceOwnerPasswordValidator.ValidateAsync),
			// where and subject was set to user id.
			Claim subjectClaim = claimsPrincipal.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);

			if (subjectClaim == null)
			{
				throw new InvalidOperationException($"Missing subject claim: '{JwtClaimTypes.Subject}'");
			}

			if (string.IsNullOrWhiteSpace(subjectClaim.Value) || !Guid.TryParse(subjectClaim.Value, out Guid userId))
			{
				throw new InvalidOperationException($"Invalid subject claim value: '{subjectClaim.Value}'. Expected a valid Guid.");
			}

			// Get user from store
			TUserIdentity user = await applicationUserManager.FindByIdAsync(userId.ToString());

			if (user == null)
			{
				throw new InvalidOperationException($"User with Id {userId} was not found in the store.");
			}

			return user;
		}

		private async Task<IEnumerable<Claim>> GetUserClaims(TUserIdentity user, IEnumerable<string> userRoles)
		{
			// when you add claims here need to whitelist them in IdentityServerConfigs
			var claims = new List<Claim>
			{
				new Claim(Claims.Keys.UserId, user.Id.ToString()),
				new Claim(Claims.Keys.FullName, $"{user.FirstName} {user.LastName}"),
				new Claim(Claims.Keys.GivenName, user.FirstName),
				new Claim(Claims.Keys.FamilyName, user.LastName),
				new Claim(Claims.Keys.Email, user.Email),
				new Claim(Claims.Keys.TenantId, user.TenantId.ToString()),
				new Claim(Claims.Keys.AuthType, Claims.Values.PasswordAuthType),
			};

			// if user roles
			if (userRoles != null)
			{
				IEnumerable<Claim> roles = userRoles.Select(role => new Claim(Claims.Keys.Role, role));
				claims.AddRange(roles);
			}

			IList<Claim> userCustomClaims = await this.applicationUserManager.GetClaimsAsync(user);
			if (userCustomClaims != null)
			{
				claims.AddRange(userCustomClaims);
			}

			return claims;
		}
	}
}
