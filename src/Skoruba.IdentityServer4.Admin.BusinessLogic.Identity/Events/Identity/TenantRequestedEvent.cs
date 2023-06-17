using Skoruba.AuditLogging.Events;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class TenantRequestedEvent<TTenantDto> : AuditEvent
    {
        public TTenantDto TenantDto { get; set; }

        public TenantRequestedEvent(TTenantDto tenantDto)
        {
            this.TenantDto = tenantDto;
        }
    }
}