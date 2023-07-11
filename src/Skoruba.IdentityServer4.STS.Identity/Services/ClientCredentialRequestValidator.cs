using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using IdentityServer4.Models;
using IdentityServer4.Validation;

using Skoruba.IdentityServer4.STS.Identity.Constants;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    /// <summary>
    /// Client credential flow validation and augmentation.
    /// </summary>
    public class ClientCredentialRequestValidator : ICustomTokenRequestValidator
    {
        /// <inheritdoc />
        public Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            ValidatedTokenRequest validatedTokenRequest = context?.Result?.ValidatedRequest;

            if (validatedTokenRequest?.GrantType == GrantType.ClientCredentials)
            {
                // if the flow is client credential, we want to add tenant Id of system and authentication type 
                // of client credentials as claims
                var claims = new List<Claim>
                {
                    new Claim(Claims.Keys.TenantId, Skoruba.IdentityServer4.Shared.Constants.SystemTenant.Id.ToString()),
                    new Claim(Claims.Keys.AuthType, Claims.Values.ClientCredentialsAuthType),
                };

                claims.ToList().ForEach(claim => context.Result.ValidatedRequest.ClientClaims.Add(claim));

                // don't want it to be prefixed with "client_" ? we change it here (or from global settings)
                context.Result.ValidatedRequest.Client.ClientClaimsPrefix = "";
            }

            return Task.CompletedTask;
        }
    }
}