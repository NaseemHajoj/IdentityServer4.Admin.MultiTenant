using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration
{
    public class MapperConfigurationBuilder : IMapperConfigurationBuilder
    {
        public HashSet<Type> ProfileTypes { get; } = new HashSet<Type>();

        public IMapperConfigurationBuilder AddProfilesType(HashSet<Type> profileTypes)
        {
            if (profileTypes == null) return this;

            foreach (var profileType in profileTypes)
            {
                ProfileTypes.Add(profileType);
            }

            return this;
        }

        public IMapperConfigurationBuilder UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole, 
            TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto>()
            where TUserDto : UserDto<string>
            where TRoleDto : RoleDto<string>
            where TUser : ApplicationUser<string>
            where TRole : IdentityRole<string>
            where TUserClaim : IdentityUserClaim<string>
            where TUserRole : IdentityUserRole<string>
            where TUserLogin : IdentityUserLogin<string>
            where TRoleClaim : IdentityRoleClaim<string>
            where TUserToken : IdentityUserToken<string>
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
            ProfileTypes.Add(typeof(IdentityMapperProfile<TUserDto, TRoleDto, TUser, TRole,
                TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>));

            return this;
        }
    }
}