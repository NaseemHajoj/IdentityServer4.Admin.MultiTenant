﻿using Skoruba.AuditLogging.Events;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.Identity
{
    public class TenantCreatedEvent<TTenantDto> : AuditEvent
        where TTenantDto : TenantDto
    {
        public TTenantDto Tenant { get; set; }

        public TenantCreatedEvent(TTenantDto tenant)
        {
            this.Tenant = tenant;
        }
    }
}