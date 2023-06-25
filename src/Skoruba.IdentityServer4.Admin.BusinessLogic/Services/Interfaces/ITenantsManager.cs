using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces
{
	public interface ITenantsManager<TUser>
		where TUser : ApplicationUser<string>, new()
    {
		Task<TenantDto> CreateNewTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default);

		Task<TenantDto> UpdateTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default);

		Task DeleteTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default);

		Task<TenantDto> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default);

		Task<TenantsDto> GetTenantsAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default);
	}
}
