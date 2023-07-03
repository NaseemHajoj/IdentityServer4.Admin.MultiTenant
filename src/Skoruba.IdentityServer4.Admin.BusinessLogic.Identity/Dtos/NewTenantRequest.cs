namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos
{
    public class NewTenantRequest
    {
        public string TenantName { get; set; }

        public string Description { get; set; }

        public string TenantAdminEmail { get; set; }

        public string TenantAdminUsername { get; set; }
    }
}
