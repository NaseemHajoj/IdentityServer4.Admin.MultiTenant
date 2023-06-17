using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class TenantUpdatedEvent<TTenantDto> : AuditEvent
        where TTenantDto : TenantDto
    {
        public TTenantDto OriginalTenant { get; set; }

        public TTenantDto Tenant { get; set; }

        public TenantUpdatedEvent(TTenantDto originalTenant, TTenantDto tenant)
        {
            this.OriginalTenant = originalTenant;
            this.Tenant = tenant;
        }
    }
}