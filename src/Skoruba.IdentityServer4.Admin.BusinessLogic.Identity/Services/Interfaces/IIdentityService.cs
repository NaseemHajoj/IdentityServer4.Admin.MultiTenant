using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces
{
    public interface IIdentityService<TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole,
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>
        where TUserDto : UserDto<string>
        where TUser : IdentityUser<string>
        where TRole : IdentityRole<string>
        where TUserClaim : IdentityUserClaim<string>
        where TUserRole : IdentityUserRole<string>
        where TUserLogin : IdentityUserLogin<string>
        where TRoleClaim : IdentityRoleClaim<string>
        where TUserToken : IdentityUserToken<string>
        where TRoleDto : RoleDto<string>
        where TUsersDto : UsersDto<TUserDto, string>
        where TRolesDto : RolesDto<TRoleDto, string>
        where TUserRolesDto : UserRolesDto<TRoleDto, string>
        where TUserClaimsDto : UserClaimsDto<TUserClaimDto, string>
        where TUserProviderDto : UserProviderDto<string>
        where TUserProvidersDto : UserProvidersDto<TUserProviderDto, string>
        where TUserChangePasswordDto : UserChangePasswordDto<string>
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, string>
        where TUserClaimDto : UserClaimDto<string>
        where TRoleClaimDto : RoleClaimDto<string>
    {
        Task<bool> ExistsUserAsync(string userId);

        Task<bool> ExistsRoleAsync(string roleId);

        
        Task<TUsersDto> GetUsersAsync(string search, int page = 1, int pageSize = 10);
        Task<TUsersDto> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10);
        Task<TUsersDto> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10);

        Task<TRolesDto> GetRolesAsync(string search, int page = 1, int pageSize = 10);

        Task<(IdentityResult identityResult, string roleId)> CreateRoleAsync(TRoleDto role);

        Task<TRoleDto> GetRoleAsync(string roleId);

        Task<List<TRoleDto>> GetRolesAsync();

        Task<(IdentityResult identityResult, string roleId)> UpdateRoleAsync(TRoleDto role);

        Task<TUserDto> GetUserAsync(string userId);

        Task<TenantsDto> GetTenantsAsync(string search, int page = 1, int pageSize = 10);

        Task<TenantDto> GetTenantAsync(Guid tenantId);

        Task<(IdentityResult identityResult, Guid tenantId)> CreateTenantAsync(TenantDto tenant);

        Task<(IdentityResult identityResult, Guid tenantId)> UpdateTenantAsync(TenantDto tenant);

        Task<IdentityResult> DeleteTenantAsync(TenantDto tenant);

        Task<(IdentityResult identityResult, string userId)> CreateUserAsync(TUserDto user);

        Task<(IdentityResult identityResult, string userId)> UpdateUserAsync(TUserDto user);

        Task<IdentityResult> DeleteUserAsync(string userId, TUserDto user);

        Task<IdentityResult> CreateUserRoleAsync(TUserRolesDto role);

        Task<TUserRolesDto> BuildUserRolesViewModel(string id, int? page);

        Task<TUserRolesDto> GetUserRolesAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<IdentityResult> DeleteUserRoleAsync(TUserRolesDto role);

        Task<TUserClaimsDto> GetUserClaimsAsync(string userId, int page = 1,
            int pageSize = 10);

        Task<TUserClaimsDto> GetUserClaimAsync(string userId, int claimId);

        Task<IdentityResult> CreateUserClaimsAsync(TUserClaimsDto claimsDto);

        Task<IdentityResult> UpdateUserClaimsAsync(TUserClaimsDto claimsDto);

        Task<IdentityResult> DeleteUserClaimAsync(TUserClaimsDto claim);

        Task<TUserProvidersDto> GetUserProvidersAsync(string userId);

        string ConvertToKeyFromString(string id);

        Task<IdentityResult> DeleteUserProvidersAsync(TUserProviderDto provider);

        Task<TUserProviderDto> GetUserProviderAsync(string userId, string providerKey);

        Task<IdentityResult> UserChangePasswordAsync(TUserChangePasswordDto userPassword);

        Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaimsDto claimsDto);

        Task<IdentityResult> UpdateRoleClaimsAsync(TRoleClaimsDto claimsDto);

        Task<TRoleClaimsDto> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10);

        Task<TRoleClaimsDto> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10);

        Task<TRoleClaimsDto> GetRoleClaimAsync(string roleId, int claimId);

        Task<IdentityResult> DeleteRoleClaimAsync(TRoleClaimsDto role);

        Task<IdentityResult> DeleteRoleAsync(TRoleDto role);
    }
}