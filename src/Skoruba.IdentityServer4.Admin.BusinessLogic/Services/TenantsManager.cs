using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.Extensions.Logging;

using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class TenantsManager : ITenantsManager
	{
		private readonly ITenantsRepository tenantsRepository;

		private readonly IMapper mapper;

		private readonly IAuditEventLogger auditEventLogger;

		private readonly ILogger<TenantsManager> logger;

		public TenantsManager(
			ITenantsRepository tenantsRepository,
			IMapper mapper,
			IAuditEventLogger auditEventLogger,
			ILogger<TenantsManager> logger) 
		{
			this.tenantsRepository = tenantsRepository;
			this.mapper = mapper;
			this.auditEventLogger = auditEventLogger;
			this.logger = logger;
		}

		public async Task<TenantDto> CreateNewTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default)
		{
            var tenant = this.mapper.Map<Tenant>(tenantDto);

            tenant = await this.tenantsRepository.CreateTenantAsync(tenant, cancellationToken);

			// get the newly created tenant dto
            TenantDto newlyCreatedTenant = this.mapper.Map<TenantDto>(tenant);

            await this.auditEventLogger.LogEventAsync(new TenantCreatedEvent<TenantDto>(newlyCreatedTenant));

            return newlyCreatedTenant;
		}

		public async Task<TenantDto> UpdateTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default)
		{
            // build the tenant model
            var tenant = this.mapper.Map<Tenant>(tenantDto);

            // do the update
            tenant = await this.tenantsRepository.UpdateTenantAsync(tenant, cancellationToken);

            TenantDto updatedTenantDto = this.mapper.Map<TenantDto>(tenant);

            await this.auditEventLogger.LogEventAsync(new TenantUpdatedEvent<TenantDto>(tenantDto, updatedTenantDto));

            return updatedTenantDto;
        }

		public async Task DeleteTenantAsync(TenantDto tenantDto, CancellationToken cancellationToken = default)
		{
            await this.tenantsRepository.DeleteTenantAsync(tenantDto.Id, cancellationToken);

            await this.auditEventLogger.LogEventAsync(new TenantDeletedEvent<TenantDto>(tenantDto));
        }

		public async Task<TenantDto> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
		{
			Tenant tenant = await this.tenantsRepository.GetTenantAsync(tenantId, cancellationToken);

			if (tenant == null)
			{
				this.logger.LogDebug("Tenant with id {TenantId} was not found", tenantId);
				return default;
			}

			var tenantDto = this.mapper.Map<TenantDto>(tenant);

			await this.auditEventLogger.LogEventAsync(new TenantRequestedEvent<TenantDto>(tenantDto));

			return tenantDto;
		}

		public async Task<TenantsDto> GetTenantsAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			PagedList<Tenant> pagedList = await this.tenantsRepository.GetTenantsAsync(search, page, pageSize, cancellationToken);
			
			var tenantsDto = this.mapper.Map<TenantsDto>(pagedList);

			await this.auditEventLogger.LogEventAsync(new UsersRequestedEvent<TenantsDto>(tenantsDto));

			return tenantsDto;
		}
	}
}
