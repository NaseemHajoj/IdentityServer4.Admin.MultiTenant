using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.DbContexts;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Services
{
    public class TenantsRepository : ITenantsRepository
	{
		private readonly AdminIdentityDbContext dbContext;

		private readonly ILogger<TenantsRepository> logger;

		public TenantsRepository(
			AdminIdentityDbContext dbContext,
			ILogger<TenantsRepository> logger) 
		{
			this.dbContext = dbContext;
			this.logger = logger;
		}

        /// <inheritdoc />
        public async Task<Tenant> CreateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
		{
			try
			{
				await this.dbContext.Tenants.AddAsync(tenant, cancellationToken);
				await this.dbContext.SaveChangesAsync(cancellationToken);

				return tenant;
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Failed to create new tenant with name \"{TenantName}\"", tenant?.Name);
				throw new Exception("Failed to create new tenant", ex);
			}
		}

        /// <inheritdoc />
        public async Task<Tenant> UpdateTenantAsync(Tenant tenant, CancellationToken cancellationToken = default)
		{
			try
			{
				// save changes to db
				this.dbContext.Tenants.Update(tenant);
				await this.dbContext.SaveChangesAsync(cancellationToken);

				return tenant;
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Failed to update tenant with id \"{TenantId}\"", tenant.Id);
				throw new Exception("Failed to update tenant", ex);
			}
		}

        /// <inheritdoc />
        public async Task DeleteTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
		{
			try
			{
				Tenant tenant = await this.GetTenantFromDatabase(tenantId, cancellationToken);

				if (tenant == null)
				{
					this.logger.LogInformation("No tenant with Id \"{TenantId}\" was found to be deleted from database", tenantId);
					return;
				}

				this.dbContext.Tenants.Remove(tenant);
				await this.dbContext.SaveChangesAsync(cancellationToken);

				this.logger.LogDebug("Tenant {TenantId} was deleted successfully", tenantId);
			}
			catch (Exception ex)
			{
				this.logger.LogError(ex, "Failed to delete tenant with id \"{TenantId}\"", tenantId);
			}
		}

        /// <inheritdoc />
        public async Task<Tenant> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
		{
			Tenant tenant = await this.GetTenantFromDatabase(tenantId, cancellationToken);

			if (tenant == null)
			{
				this.logger.LogDebug("Tenant with id {TenantId} was not found", tenantId);
				return default;
			}

			return tenant;
		}

		/// <inheritdoc />
		public async Task<PagedList<Tenant>> GetTenantsAsync(string search, int page = 1, int pageSize = 10, CancellationToken cancellationToken = default)
		{
			var pagedList = new PagedList<Tenant>();

			Expression<Func<Tenant, bool>> searchCondition = tenant => tenant.Name.Contains(search) || tenant.Email.Contains(search);

			var tenants = await this.dbContext.Tenants.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync(cancellationToken);

			pagedList.Data.AddRange(tenants);

			pagedList.TotalCount = await this.dbContext.Tenants.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync(cancellationToken);
			pagedList.PageSize = pageSize;

			return pagedList;
		}

		private Task<Tenant> GetTenantFromDatabase(Guid tenantId, CancellationToken cancellationToken = default)
		{
			return this.dbContext.Tenants.FirstOrDefaultAsync(tenant => tenant.Id == tenantId, cancellationToken);
		}
    }
}
