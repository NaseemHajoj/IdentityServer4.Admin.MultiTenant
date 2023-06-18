using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity
{
    public class ApplicationUserManager<TUser, TKey> : UserManager<TUser>
        where TUser : ApplicationUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        private readonly IServiceProvider _services;

        public ApplicationUserManager(
            ApplicationUserStore<TUser, TKey> store, 
            IOptions<IdentityOptions> optionsAccessor, 
            IPasswordHasher<TUser> passwordHasher, 
            IEnumerable<IUserValidator<TUser>> userValidators, 
            IEnumerable<IPasswordValidator<TUser>> passwordValidators, 
            ILookupNormalizer keyNormalizer, 
            IdentityErrorDescriber errors, 
            IServiceProvider services, 
            ILogger<UserManager<TUser>> logger) 
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
            this._services = services;
        }

        public async Task<TUser> FindByEmailAsync(string email, string tenant)
        {
            this.ThrowIfDisposed();
            var store = this.GetApplicationUserStore();

            email = NormalizeEmail(email);
            var user = await store.FindByEmailAsync(email, tenant, this.CancellationToken);

            // Need to potentially check all keys
            if (user == null && Options.Stores.ProtectPersonalData)
            {
                var keyRing = _services.GetService<ILookupProtectorKeyRing>();
                var protector = _services.GetService<ILookupProtector>();
                if (keyRing != null && protector != null)
                {
                    foreach (var key in keyRing.GetAllKeyIds())
                    {
                        var oldKey = protector.Protect(key, email);
                        user = await store.FindByEmailAsync(oldKey, CancellationToken);
                        if (user != null)
                        {
                            return user;
                        }
                    }
                }
            }

            return user;
        }

        public Task<TUser> FindByNameAsync(string userName, string tenant)
        {
            this.ThrowIfDisposed();
            if (userName == null)
            {
                throw new ArgumentNullException(nameof(userName));
            }

            return this.GetApplicationUserStore().FindByNameAsync(userName, tenant);
        }

        private ApplicationUserStore<TUser, TKey> GetApplicationUserStore()
        {
            return this.Store as ApplicationUserStore<TUser, TKey>;
        }
    }
}