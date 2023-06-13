using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Skoruba.IdentityServer4.Admin.EntityFramework.Constants;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity
{
    public class ApplicationUser : IdentityUser
	{
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        public int EntityStateId { get; set; }

        /// <summary>
        /// Gets or Sets when user was registered in system.
        /// </summary>
        public DateTime RegisterDateTime { get; set; }

        /// <summary>
        /// Gets or Sets when user was last logged in.
        /// </summary>
        public DateTime? LastLoginDateTime { get; set; }

        /// <summary>
        /// Gets or Sets when user account was marked as deleted.
        /// </summary>
        public DateTime? MarkedAsDeletedDateTime { get; set; }

        public Guid TenantId { get; set; }

        [Comment("Created timestamp in UTC")]
        public DateTime CreatedDateTime { get; set; }

        [Comment("Updated timestamp in UTC")]
        public DateTime UpdatedDateTime { get; set; }

        [ForeignKey(nameof(TenantId))]
        public Tenant Tenant { get; set; }
    }

    public class ApplicationUserEntityTypeConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(b => b.CreatedDateTime)
                .HasDefaultValueSql(DatabaseConstants.GetUtcDateSqlFunction);

            builder.Property(b => b.UpdatedDateTime)
                .HasDefaultValueSql(DatabaseConstants.GetUtcDateSqlFunction);

            // Need to have this since the base class adds an unique index on Username
            // making it impossible to create same user name in different tenants.
            // Checkout the names of the indexes in OnModelCreating here:
            // https://github.com/aspnet/Identity/blob/feedcb5c53444f716ef5121d3add56e11c7b71e5/src/EF/IdentityUserContext.cs

            var index = builder.HasIndex(u => u.NormalizedUserName).Metadata;
            builder.Metadata.RemoveIndex(index.Properties);

            index = builder.HasIndex(u => u.NormalizedEmail).Metadata;
            builder.Metadata.RemoveIndex(index.Properties);

            builder
                .HasIndex(user => new { user.NormalizedUserName, user.TenantId })
                .HasDatabaseName("UserNameIndex")
                .IsUnique();

            builder
                .HasIndex(user => new { user.NormalizedEmail, user.TenantId })
                .HasDatabaseName("EmailIndex")
                .IsUnique();
        }
    }
}