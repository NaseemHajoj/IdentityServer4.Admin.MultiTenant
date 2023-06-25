using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;

using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces
{
    public interface IIdentityRepository<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TUser : ApplicationUser<string>
        where TRole : IdentityRole<string>
        where TUserClaim : IdentityUserClaim<string>
        where TUserRole : IdentityUserRole<string>
        where TUserLogin : IdentityUserLogin<string>
        where TRoleClaim : IdentityRoleClaim<string>
        where TUserToken : IdentityUserToken<string>
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        Task<PagedList<TUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10);

        Task<PagedList<TUser>> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10);

        Task<PagedList<TUser>> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10);

        Task<PagedList<TRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, string roleId)> CreateRoleAsync(TRole role);

        Task<TRole> GetRoleAsync(string roleId);

        Task<List<TRole>> GetRolesAsync();

        Task<(IdentityResult identityResult, string roleId)> UpdateRoleAsync(TRole role);

        Task<TUser> GetUserAsync(string userId);

        Task<(IdentityResult identityResult, string userId)> CreateUserAsync(TUser user);

        Task<(IdentityResult identityResult, string userId)> UpdateUserAsync(TUser user);

        Task<IdentityResult> DeleteUserAsync(string userId);

        Task<IdentityResult> CreateUserRoleAsync(string userId, string roleId);

        Task<PagedList<TRole>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(string userId, string roleId);

        Task<PagedList<TUserClaim>> GetUserClaimsAsync(string userId, int page = 1, int pageSize = 10);

        Task<TUserClaim> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaim claims);

        Task<IdentityResult> UpdateUserClaimsAsync(TUserClaim claims);

        Task<IdentityResult> DeleteUserClaimAsync(string userId, int claimId);

        Task<List<UserLoginInfo>> GetUserProvidersAsync(string userId);

        Task<IdentityResult> DeleteUserProvidersAsync(string userId, string providerKey, string loginProvider);

        Task<TUserLogin> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(string userId, string password);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaim claims);

        Task<IdentityResult> UpdateRoleClaimsAsync(TRoleClaim claims);

        Task<PagedList<TRoleClaim>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<PagedList<TRoleClaim>> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10);

        Task<TRoleClaim> GetRoleClaimAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleClaimAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleAsync(TRole role);

        bool AutoSaveChanges { get; set; }

        Task<int> SaveAllChangesAsync();
    }
}