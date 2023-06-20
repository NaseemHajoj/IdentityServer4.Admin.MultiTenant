using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces
{
    public interface IMultitenantUserManager<TUser, TKey>
        where TUser : ApplicationUser<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        Task<TUser> FindByEmailAsync(string email, string tenant);

        Task<TUser> FindByNameAsync(string userName, string tenant);
    }

    public interface IMultitenantUserManager<TUser> : IMultitenantUserManager<TUser, string>
        where TUser : ApplicationUser<string>, new()
    {
    }
}