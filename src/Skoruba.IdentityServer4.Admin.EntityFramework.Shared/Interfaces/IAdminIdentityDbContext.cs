using System;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces
{
    public interface IApplicationUser
    {
        Guid TenantId { get; }

        string TenantName { get; }
    }
}