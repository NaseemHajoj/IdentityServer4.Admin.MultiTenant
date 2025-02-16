﻿using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity
{
    /// <summary>
    /// Extending the <see cref="UserStore{TUser}"/> class to make store aware of tenant.
    /// </summary>
    /// <typeparam name="TUser"></typeparam>
    public class ApplicationUserStore<TUser> : UserStore<TUser>, IMultitenantUserStore<TUser>
        where TUser : ApplicationUser<string>, new()
    {
        public ApplicationUserStore(
            AdminIdentityDbContext context, 
            IdentityErrorDescriber describer = null) 
            : base(context, describer)
        {
        }

        /// <inheritdoc />
        public Task<TUser> FindByNameAsync(string normalizedUserName, string tenant, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            this.ThrowIfDisposed();

            string normalizedTenantName = this.NormalizeTenantName(tenant);

            return this.Users
                .Include(o => o.Tenant)
                .SingleOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName && u.Tenant.NormalizedName == normalizedTenantName, cancellationToken);
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, string tenant,  CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();
            ThrowIfDisposed();

            string normalizedTenantName = this.NormalizeTenantName(tenant);

            return this.Users
                .Include(o => o.Tenant)
                .SingleOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail && u.Tenant.NormalizedName == normalizedTenantName, cancellationToken);
        }

        private string NormalizeTenantName(string tenantName)
        {
            return tenantName.Trim().ToUpper();
        }
    }

    public class ApplicationUserStore : ApplicationUserStore<ApplicationUser<string>>
    {
        public ApplicationUserStore(
            AdminIdentityDbContext context, 
            IdentityErrorDescriber describer = null) 
            : base(context, describer)
        {
        }
    }
}