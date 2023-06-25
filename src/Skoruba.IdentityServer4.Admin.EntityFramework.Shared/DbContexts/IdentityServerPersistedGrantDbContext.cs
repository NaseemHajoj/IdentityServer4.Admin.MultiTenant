using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;

using Microsoft.EntityFrameworkCore;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class IdentityServerPersistedGrantDbContext : PersistedGrantDbContext<IdentityServerPersistedGrantDbContext>, IAdminPersistedGrantDbContext
    {
        public IdentityServerPersistedGrantDbContext(DbContextOptions<IdentityServerPersistedGrantDbContext> options, OperationalStoreOptions storeOptions)
            : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure default schema
            modelBuilder.HasDefaultSchema(DatabaseConstants.IdentityServerConfigurationDbSchema);

            base.OnModelCreating(modelBuilder);
        }
    }
}