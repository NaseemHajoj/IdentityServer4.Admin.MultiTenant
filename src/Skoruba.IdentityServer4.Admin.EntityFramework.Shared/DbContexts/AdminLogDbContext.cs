using Microsoft.EntityFrameworkCore;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts
{
    public class AdminLogDbContext : DbContext, IAdminLogDbContext
    {
        public DbSet<Log> Logs { get; set; }

        public AdminLogDbContext(DbContextOptions<AdminLogDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Configure default schema
            builder.HasDefaultSchema(DatabaseConstants.IdentityServerConfigurationDbSchema);

            base.OnModelCreating(builder);

            ConfigureLogContext(builder);
        }

        private void ConfigureLogContext(ModelBuilder builder)
        {
            builder.Entity<Log>(log =>
            {
                log.ToTable(TableConsts.Logging);
                log.HasKey(x => x.Id);
                log.Property(x => x.Level).HasMaxLength(128);
            });
        }
    }
}