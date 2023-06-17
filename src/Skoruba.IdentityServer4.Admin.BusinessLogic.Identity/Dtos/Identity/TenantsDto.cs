using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces;

using System.Collections.Generic;
using System.Linq;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity
{
    public class TenantsDto : ITenantsDto
    {
        public TenantsDto()
        {
            this.Tenants = new List<TenantDto>();
        }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public List<TenantDto> Tenants { get; set; }

        List<ITenantDto> ITenantsDto.Tenants => Tenants.Cast<ITenantDto>().ToList();
    }
}
