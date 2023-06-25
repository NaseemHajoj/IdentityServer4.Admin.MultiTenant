// Based on the IdentityServer4.EntityFramework - authors - Brock Allen & Dominick Baier.
// https://github.com/IdentityServer/IdentityServer4.EntityFramework

// Modified by Jan Škoruba

using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Identity;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers
{
    public class IdentityMapperProfile<TUserDto, TRoleDto, TUser, TRole, TUserClaim, TUserRole,
        TUserLogin, TRoleClaim, TUserToken,
        TUsersDto, TRolesDto, TUserRolesDto, TUserClaimsDto,
        TUserProviderDto, TUserProvidersDto, TRoleClaimsDto,
        TUserClaimDto, TRoleClaimDto>
        : Profile
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
        where TRoleClaimsDto : RoleClaimsDto<TRoleClaimDto, string>
        where TUserClaimDto : UserClaimDto<string>
        where TRoleClaimDto : RoleClaimDto<string>
    {
        public IdentityMapperProfile()
        {
            // entity to model
            CreateMap<TUser, TUserDto>(MemberList.Destination)
                .ForMember(dest => dest.TenantName, opt => opt.MapFrom(src => src.Tenant.Name));

            CreateMap<Tenant, TenantDto>(MemberList.Destination);

            CreateMap<UserLoginInfo, TUserProviderDto>(MemberList.Destination);

            CreateMap<IdentityError, ViewErrorMessage>(MemberList.Destination)
                .ForMember(x => x.ErrorKey, opt => opt.MapFrom(src => src.Code))
                .ForMember(x => x.ErrorMessage, opt => opt.MapFrom(src => src.Description));

            // entity to model
            CreateMap<TRole, TRoleDto>(MemberList.Destination);

            CreateMap<TUser, TUser>(MemberList.Destination)
                .ForMember(x => x.SecurityStamp, opt => opt.Ignore());

            CreateMap<TRole, TRole>(MemberList.Destination);

            CreateMap<PagedList<TUser>, TUsersDto>(MemberList.Destination)
                .ForMember(x => x.Users,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<Tenant>, TenantsDto>(MemberList.Destination)
                .ForMember(x => x.Tenants,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<TUserClaim, TUserClaimDto>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TUserClaim, TUserClaimsDto>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<PagedList<TRole>, TRolesDto>(MemberList.Destination)
                .ForMember(x => x.Roles,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<TRole>, TUserRolesDto>(MemberList.Destination)
                .ForMember(x => x.Roles,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<PagedList<TUserClaim>, TUserClaimsDto>(MemberList.Destination)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Data));
            
            CreateMap<PagedList<TRoleClaim>, TRoleClaimsDto>(MemberList.Destination)
                .ForMember(x => x.Claims,
                    opt => opt.MapFrom(src => src.Data));

            CreateMap<List<UserLoginInfo>, TUserProvidersDto>(MemberList.Destination)
                .ForMember(x => x.Providers, opt => opt.MapFrom(src => src));

            CreateMap<UserLoginInfo, TUserProviderDto>(MemberList.Destination);

            CreateMap<TRoleClaim, TRoleClaimDto>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TRoleClaim, TRoleClaimsDto>(MemberList.Destination)
                .ForMember(x => x.ClaimId, opt => opt.MapFrom(src => src.Id));

            CreateMap<TUserLogin, TUserProviderDto>(MemberList.Destination);

            // model to entity
            CreateMap<TRoleDto, TRole>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Condition(srs => srs.Id != null)); ;

            CreateMap<TRoleClaimsDto, TRoleClaim>(MemberList.Source);

            CreateMap<TUserClaimsDto, TUserClaim>(MemberList.Source)
                .ForMember(x => x.Id,
                    opt => opt.MapFrom(src => src.ClaimId));

            // model to entity
            CreateMap<TUserDto, TUser>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Condition(srs => srs.Id != null));

            CreateMap<TenantDto, Tenant>(MemberList.Source)
                .ForMember(dest => dest.Id, opt => opt.Condition(srs => srs.Id != Guid.Empty))
                .ForMember(dest => dest.NormalizedName, opt => opt.MapFrom(src => src.Name.ToUpper()));
        }
    }
}