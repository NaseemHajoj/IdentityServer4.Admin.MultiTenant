using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;

using System;
using System.ComponentModel.DataAnnotations;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Entities
{
    public class Tenant
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Comment("Created timestamp in UTC")]
        public DateTime CreatedDateTime { get; set; }

        [Comment("Updated timestamp in UTC")]
        public DateTime UpdatedDateTime { get; set; }
    }

    public class TenantEntityTypeConfiguration : IEntityTypeConfiguration<Tenant>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            builder.Property(b => b.CreatedDateTime)
                .HasDefaultValueSql(DatabaseConstants.GetUtcDateSqlFunction);

            builder.Property(b => b.UpdatedDateTime)
                .HasDefaultValueSql(DatabaseConstants.GetUtcDateSqlFunction);

            builder.HasIndex(tenant => tenant.Name)
                .IsUnique();
        }
    }
}