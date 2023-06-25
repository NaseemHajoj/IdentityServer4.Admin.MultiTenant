using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;

using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.STS.Identity.Services
{
    public class TenantStore : ITenantStore
	{
		private const int CacheTimeToLiveInMinutes = 10;

		private readonly ITenantsRepository tenantsRepository;

		/// <summary>
		/// A memory cache used to cache tenants
		/// </summary>
		private readonly IMemoryCache memoryCache;

		public TenantStore(
            ITenantsRepository tenantsRepository,
			IMemoryCache memoryCache) {

			this.tenantsRepository = tenantsRepository;
			this.memoryCache = memoryCache;
		}

		/// <inheritdoc />
		public async Task<Tenant> GetTenantAsync(Guid tenantId, CancellationToken cancellationToken)
		{
			var stopwatch = Stopwatch.StartNew();
			Tenant tenant = null;

			bool operationSucceeded = false;

			try
			{
				string tenantCacheKey = this.BuildTenantCacheKey(tenantId);

				if (!this.memoryCache.TryGetValue(tenantCacheKey, out tenant))
				{
					tenant = await this.tenantsRepository.GetTenantAsync(tenantId, cancellationToken);
					this.memoryCache.Set(tenantCacheKey, tenant, TimeSpan.FromMinutes(CacheTimeToLiveInMinutes));
				}

				// if the above line threw an exception, the flag will remain false
				operationSucceeded = true;
			}
			finally
			{
				stopwatch.Stop();
			}

			return tenant;
		}

		/// <summary>
		/// Building a tenant cache key given the <paramref name="tenantId"/>.
		/// </summary>
		/// <returns></returns>
		private string BuildTenantCacheKey(Guid tenantId)
		{
			return $"TenantStore_{tenantId.ToString().ToLowerInvariant()}";
		}
	}
}