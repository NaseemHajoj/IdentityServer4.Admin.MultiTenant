using System.Collections.Generic;
using Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Configuration.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Configuration.Configuration
{
	public class IdentityData
    {
       public Tenant SystemTenant { get; set; }
       public List<Role> Roles { get; set; }
       public List<User> Users { get; set; }
    }
}
