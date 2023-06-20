using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces
{
    public interface IMultitenantUserStore<TUser> : IUserStore<TUser>
        where TUser : ApplicationUser<string>
    {
        Task<TUser> FindByNameAsync(string normalizedUserName, string tenant, CancellationToken cancellationToken = default(CancellationToken));

        Task<TUser> FindByEmailAsync(string normalizedEmail, string tenant, CancellationToken cancellationToken = default(CancellationToken));
    }
}