using System;
using System.Threading;
using System.Threading.Tasks;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    public interface ITenantStore
	{
		/// <summary>
		/// Get a tenant for the given tenant Id <paramref name="tenantId"/>
		/// </summary>
		/// <param name="tenantId">The tenant Id.</param>
		/// <returns>The tenant.</returns>
		Task<Tenant> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken);
	}
}
