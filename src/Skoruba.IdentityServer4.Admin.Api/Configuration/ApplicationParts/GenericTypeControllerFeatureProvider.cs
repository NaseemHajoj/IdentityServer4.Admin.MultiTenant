using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;

namespace Skoruba.IdentityServer4.Admin.Api.Configuration.ApplicationParts
{
    public class GenericTypeControllerFeatureProvider<TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto> : IApplicationFeatureProvider<ControllerFeature>
        where TUserDto : UserDto<string>, new()
        where TRoleDto : RoleDto<string>, new()
        where TUser : IdentityUser<string>
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
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var currentAssembly = typeof(GenericTypeControllerFeatureProvider<TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>).Assembly;
            var controllerTypes = currentAssembly.GetExportedTypes()
                                                 .Where(t => typeof(ControllerBase).IsAssignableFrom(t) && t.IsGenericTypeDefinition)
                                                 .Select(t => t.GetTypeInfo());

            var type = this.GetType();
            var genericType = type.GetGenericTypeDefinition().GetTypeInfo();
            var parameters = genericType.GenericTypeParameters
                                        .Select((p, i) => new { p.Name, Index = i })
                                        .ToDictionary(a => a.Name, a => type.GenericTypeArguments[a.Index]);

            foreach (var controllerType in controllerTypes)
            {
                var typeArguments = controllerType.GenericTypeParameters
                                                  .Select(p => parameters[p.Name])
                                                  .ToArray();

                feature.Controllers.Add(controllerType.MakeGenericType(typeArguments).GetTypeInfo());
            }
        }
    }
}