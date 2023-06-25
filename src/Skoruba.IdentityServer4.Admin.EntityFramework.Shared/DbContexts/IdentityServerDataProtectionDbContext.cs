using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class IdentityServerDataProtectionDbContext : DbContext, IDataProtectionKeyContext
    {
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

        public IdentityServerDataProtectionDbContext(DbContextOptions<IdentityServerDataProtectionDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure default schema
            modelBuilder.HasDefaultSchema(DatabaseConstants.IdentityServerConfigurationDbSchema);

            base.OnModelCreating(modelBuilder);
        }
    }
}