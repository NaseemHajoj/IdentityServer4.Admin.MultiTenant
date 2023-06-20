using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AdminServicesExtensions
    {
        public static IMapperConfigurationBuilder AddAdminAspNetIdentityMapping(this IServiceCollection services)
        {
            var builder = new MapperConfigurationBuilder();

            services.AddSingleton<IConfigurationProvider>(sp => new MapperConfiguration(cfg =>
            {
                foreach (var profileType in builder.ProfileTypes)
                    cfg.AddProfile(profileType);
            }));

            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<IConfigurationProvider>(), sp.GetService));

            return builder;
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUser>(
            this IServiceCollection services)
            where TIdentityDbContext : IdentityDbContext<TUser, IdentityRole, string, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>, IAdminIdentityDbContext
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TUser : ApplicationUser<string>, new()
        {
            return services.AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, UserDto<string>, RoleDto<string>,
                TUser, IdentityRole, IdentityUserClaim<string>, IdentityUserRole<string>, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>,
                UsersDto<UserDto<string>, string>, RolesDto<RoleDto<string>, string>, UserRolesDto<RoleDto<string>, string>,
                UserClaimsDto<UserClaimDto<string>, string>, UserProviderDto<string>, UserProvidersDto<UserProviderDto<string>, string>, UserChangePasswordDto<string>,
                RoleClaimsDto<RoleClaimDto<string>, string>, UserClaimDto<string>, RoleClaimDto<string>>();
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TAdminDbContext, TUserDto, TUserDtoKey, TRoleDto, TRoleDtoKey,
            TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
            TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
            TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto,
            TUserClaimDto, TRoleClaimDto>(
                this IServiceCollection services)
            where TAdminDbContext :
            IdentityDbContext<TUser, TRole, string, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, 
            IAdminIdentityDbContext,
            IAdminPersistedGrantDbContext
            where TUserDto : UserDto<string>
            where TUser : ApplicationUser<string>, new()
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

            return services.AddAdminAspNetIdentityServices<TAdminDbContext, TAdminDbContext, TUserDto, TRoleDto,
                TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>();
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
                        this IServiceCollection services)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, string, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IAdminIdentityDbContext
            where TUserDto : UserDto<string>
            where TUser : ApplicationUser<string>, new()
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
            return AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto,
                TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin,
                TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
                TRoleClaimDto>(services, null);
        }

        public static IServiceCollection AddAdminAspNetIdentityServices<TIdentityDbContext, TPersistedGrantDbContext, TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>(
                        this IServiceCollection services, HashSet<Type> profileTypes)
            where TPersistedGrantDbContext : DbContext, IAdminPersistedGrantDbContext
            where TIdentityDbContext : IdentityDbContext<TUser, TRole, string, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IAdminIdentityDbContext
            where TUserDto : UserDto<string>
            where TUser : ApplicationUser<string>, new()
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
            //Repositories
            services.AddTransient<IIdentityRepository<TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IdentityRepository<TIdentityDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
            services.AddTransient<IPersistedGrantAspNetIdentityRepository, PersistedGrantAspNetIdentityRepository<TIdentityDbContext, TPersistedGrantDbContext, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>>();
          
            //Services
            services.AddTransient<IIdentityService<TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>, 
                IdentityService<TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto, TRoleClaimDto>>();
            services.AddTransient<IPersistedGrantAspNetIdentityService, PersistedGrantAspNetIdentityService>();
            
            //Resources
            services.AddScoped<IIdentityServiceResources, IdentityServiceResources>();
            services.AddScoped<IPersistedGrantAspNetIdentityServiceResources, PersistedGrantAspNetIdentityServiceResources>();

            //Register mapping
            services.AddAdminAspNetIdentityMapping()
                .UseIdentityMappingProfile<TUserDto, TRoleDto, TUser, TRole, TUserClaim,
                    TUserRole, TUserLogin, TRoleClaim, TUserToken,
                    TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
                    TUserProviderDto, TUserProvidersDto, TUserChangePasswordDto, TRoleClaimsDto, TUserClaimDto,
                    TRoleClaimDto>()
                .AddProfilesType(profileTypes);

            return services;
        }
    }
}
