﻿using System;
using System.Threading.Tasks;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity;
using Skoruba.IdentityServer4.Shared.Configuration.Configuration.Identity;

namespace Skoruba.IdentityServer4.STS.Identity.Helpers
{
    public class UserResolver<TUser> 
        where TUser : ApplicationUser<string>, new()
    {
        private readonly ApplicationUserManager<TUser> _userManager;
        private readonly LoginResolutionPolicy _policy;

        public UserResolver(ApplicationUserManager<TUser> userManager, LoginConfiguration configuration)
        {
            _userManager = userManager;
            _policy = configuration.ResolutionPolicy;
        }

        public async Task<TUser> GetUserAsync(string login, string tenantName)
        {
            switch (_policy)
            {
                case LoginResolutionPolicy.Username:
                    return await _userManager.FindByNameAsync(login, tenantName);
                case LoginResolutionPolicy.Email:
                    return await _userManager.FindByEmailAsync(login, tenantName);
                default:
                    return null;
            }
        }
    }
}
