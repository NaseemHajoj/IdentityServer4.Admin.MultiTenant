using System;

using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces
{
    public interface IAdminIdentityDbContext
    {
        DbSet<Tenant> Tenants { get; set; }
    }
}