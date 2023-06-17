using System.Collections.Generic;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface ITenantsDto
    {
        int PageSize { get; set; }
        int TotalCount { get; set; }
        List<ITenantDto> Tenants { get; }
    }
}
