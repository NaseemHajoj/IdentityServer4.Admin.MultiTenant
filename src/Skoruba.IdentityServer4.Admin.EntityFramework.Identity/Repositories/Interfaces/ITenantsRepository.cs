using System;
using System.Threading;
using System.Threading.Tasks;

using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces
{
    public interface ITenantsRepository
	{
		Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);

		Task<Tenant> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default);

		Task DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

		Task<Tenant> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

		Task<PagedList<Tenant>> GetTenantsAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
	}
}
