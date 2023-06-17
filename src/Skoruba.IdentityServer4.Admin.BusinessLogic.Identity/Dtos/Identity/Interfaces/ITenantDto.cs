using System;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity.Interfaces
{
    public interface ITenantDto
    {
        Guid Id { get; set; }

        string Name { get; set; }

        string Description { get; set; }

        string Email { get; set; }

        bool IsActive { get; set; }

        DateTime CreatedDateTime { get; set; }
    }
}
