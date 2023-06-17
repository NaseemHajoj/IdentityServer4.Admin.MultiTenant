﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Extensions;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Enums;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Interfaces;

namespace Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories
{
    public class IdentityRepository<TIdentityDbContext, TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        : IIdentityRepository<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
        where TIdentityDbContext : IdentityDbContext<TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>, IAdminIdentityDbContext
        where TUser : ApplicationUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TRoleClaim : IdentityRoleClaim<TKey>
        where TUserToken : IdentityUserToken<TKey>
    {
        protected readonly TIdentityDbContext DbContext;
        protected readonly UserManager<TUser> UserManager;
        protected readonly RoleManager<TRole> RoleManager;
        protected readonly IMapper Mapper;

        public bool AutoSaveChanges { get; set; } = true;

        public IdentityRepository(TIdentityDbContext dbContext,
            UserManager<TUser> userManager,
            RoleManager<TRole> roleManager,
            IMapper mapper)
        {
            DbContext = dbContext;
            UserManager = userManager;
            RoleManager = roleManager;
            Mapper = mapper;
        }

        public virtual TKey ConvertKeyFromString(string id)
        {
            if (id == null)
            {
                return default;
            }
            return (TKey)TypeDescriptor.GetConverter(typeof(TKey)).ConvertFromInvariantString(id);
        }

        public virtual Task<bool> ExistsUserAsync(string userId)
        {
            var id = ConvertKeyFromString(userId);

            return UserManager.Users.AnyAsync(x => x.Id.Equals(id));
        }

        public virtual Task<bool> ExistsRoleAsync(string roleId)
        {
            var id = ConvertKeyFromString(roleId);

            return RoleManager.Roles.AnyAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<PagedList<Tenant>> GetTenantsAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<Tenant>();
            Expression<Func<Tenant, bool>> searchCondition = tenant => tenant.Name.Contains(search) || tenant.Email.Contains(search);

            var tenants = await this.DbContext.Tenants.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(tenants);

            pagedList.TotalCount = await this.DbContext.Tenants.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<Tenant> GetTenantAsync(Guid tenantId)
        {
            return this.DbContext.Tenants.FirstOrDefaultAsync(tenant => tenant.Id == tenantId);
        }

        public virtual async Task<(IdentityResult identityResult, Guid tenantId)> CreateTenantAsync(Tenant tenant)
        {
            try
            {
                await this.DbContext.Tenants.AddAsync(tenant);
                await this.DbContext.SaveChangesAsync();

                return (IdentityResult.Success, tenant.Id);
            }
            catch (Exception ex)
            {
                var errors = new IdentityError
                {
                    Code = "TenantCreationFailed",
                    Description = ex.Message,
                };

                return (IdentityResult.Failed(errors), Guid.Empty);
            }
        }

        public virtual async Task<(IdentityResult identityResult, Guid tenantId)> UpdateTenantAsync(Tenant updatedTenant)
        {
            try
            {
                var tenant = await this.DbContext.Tenants.FirstOrDefaultAsync(t => t.Id == updatedTenant.Id);
                this.Mapper.Map(updatedTenant, tenant);
                this.DbContext.Tenants.Update(tenant);
                await this.DbContext.SaveChangesAsync();

                return (IdentityResult.Success, tenant.Id);
            }
            catch (Exception ex)
            {
                var errors = new IdentityError
                {
                    Code = "TenantUpdateFailed",
                    Description = ex.Message,
                };

                return (IdentityResult.Failed(errors), Guid.Empty);
            }
        }

        public virtual async Task<IdentityResult> DeleteTenantAsync(Guid tenantId)
        {
            try
            {
                Tenant tenant = await this.DbContext.Tenants.FirstOrDefaultAsync(tenant => tenant.Id == tenantId);

                if (tenant != null)
                {
                    this.DbContext.Tenants.Remove(tenant);
                    await this.DbContext.SaveChangesAsync();
                }

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                var errors = new IdentityError
                {
                    Code = "TenantDeletionFailed",
                    Description = ex.Message,
                };

                return IdentityResult.Failed(errors);
            }
        }

        public virtual async Task<PagedList<TUser>> GetUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TUser>();
            Expression<Func<TUser, bool>> searchCondition = x => x.UserName.Contains(search) || x.Email.Contains(search) || x.TenantName.Contains(search);

            var users = await UserManager.Users
                .Include(o => o.Tenant)
                .WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(users);

            pagedList.TotalCount = await UserManager.Users.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<TUser>> GetRoleUsersAsync(string roleId, string search, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(roleId);

            var pagedList = new PagedList<TUser>();
            var users = DbContext.Set<TUser>()
                .Join(DbContext.Set<TUserRole>(), u => u.Id, ur => ur.UserId, (u, ur) => new { u, ur })
                .Where(t => t.ur.RoleId.Equals(id))
                .WhereIf(!string.IsNullOrEmpty(search), t => t.u.UserName.Contains(search) || t.u.Email.Contains(search))
                .Select(t => t.u);

            var pagedUsers = await users.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(pagedUsers);
            pagedList.TotalCount = await users.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<TUser>> GetClaimUsersAsync(string claimType, string claimValue, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TUser>();
            var users = DbContext.Set<TUser>()
                .Join(DbContext.Set<TUserClaim>(), u => u.Id, uc => uc.UserId, (u, uc) => new { u, uc })
                .Where(t => t.uc.ClaimType.Equals(claimType))
                .WhereIf(!string.IsNullOrEmpty(claimValue), t => t.uc.ClaimValue.Equals(claimValue))
                .Select(t => t.u).Distinct();

            var pagedUsers = await users.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(pagedUsers);
            pagedList.TotalCount = await users.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<List<TRole>> GetRolesAsync()
        {
            return RoleManager.Roles.ToListAsync();
        }

        public virtual async Task<PagedList<TRole>> GetRolesAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = new PagedList<TRole>();

            Expression<Func<TRole, bool>> searchCondition = x => x.Name.Contains(search);
            var roles = await RoleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).PageBy(x => x.Id, page, pageSize).ToListAsync();

            pagedList.Data.AddRange(roles);
            pagedList.TotalCount = await RoleManager.Roles.WhereIf(!string.IsNullOrEmpty(search), searchCondition).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<TRole> GetRoleAsync(TKey roleId)
        {
            return RoleManager.Roles.Where(x => x.Id.Equals(roleId)).SingleOrDefaultAsync();
        }

        public virtual async Task<(IdentityResult identityResult, TKey roleId)> CreateRoleAsync(TRole role)
        {
            var identityResult = await RoleManager.CreateAsync(role);

            return (identityResult, role.Id);
        }

        public virtual async Task<(IdentityResult identityResult, TKey roleId)> UpdateRoleAsync(TRole role)
        {
            var existingRole = await RoleManager.FindByIdAsync(role.Id.ToString());
            Mapper.Map(role, existingRole);
            var identityResult = await RoleManager.UpdateAsync(existingRole);

            return (identityResult, role.Id);
        }

        public virtual async Task<IdentityResult> DeleteRoleAsync(TRole role)
        {
            var thisRole = await RoleManager.FindByIdAsync(role.Id.ToString());

            return await RoleManager.DeleteAsync(thisRole);
        }

        public virtual async Task<TUser> GetUserAsync(string userId)
        {
            TUser user = await UserManager.FindByIdAsync(userId);
            
            this.DbContext.Entry(user)
                .Reference(o => o.Tenant)
                .Load();

            return user;
        }

        /// <summary>
        /// Create a new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns>This method returns identity result and new user id</returns>
        public virtual async Task<(IdentityResult identityResult, TKey userId)> CreateUserAsync(TUser user)
        {
            var identityResult = await UserManager.CreateAsync(user);

            return (identityResult, user.Id);
        }

        public virtual async Task<(IdentityResult identityResult, TKey userId)> UpdateUserAsync(TUser user)
        {
            var userIdentity = await UserManager.FindByIdAsync(user.Id.ToString());
            Mapper.Map(user, userIdentity);
            var identityResult = await UserManager.UpdateAsync(userIdentity);

            return (identityResult, user.Id);
        }

        public virtual async Task<IdentityResult> CreateUserRoleAsync(string userId, string roleId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var selectRole = await RoleManager.FindByIdAsync(roleId);

            return await UserManager.AddToRoleAsync(user, selectRole.Name);
        }

        public virtual async Task<PagedList<TRole>> GetUserRolesAsync(string userId, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(userId);

            var pagedList = new PagedList<TRole>();
            var roles = from r in DbContext.Set<TRole>()
                        join ur in DbContext.Set<TUserRole>() on r.Id equals ur.RoleId
                        where ur.UserId.Equals(id)
                        select r;

            var userIdentityRoles = await roles.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(userIdentityRoles);
            pagedList.TotalCount = await roles.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<IdentityResult> DeleteUserRoleAsync(string userId, string roleId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);
            var user = await UserManager.FindByIdAsync(userId);

            return await UserManager.RemoveFromRoleAsync(user, role.Name);
        }

        public virtual async Task<PagedList<TUserClaim>> GetUserClaimsAsync(string userId, int page, int pageSize)
        {
            var id = ConvertKeyFromString(userId);
            var pagedList = new PagedList<TUserClaim>();

            var claims = await DbContext.Set<TUserClaim>().Where(x => x.UserId.Equals(id))
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await DbContext.Set<TUserClaim>().Where(x => x.UserId.Equals(id)).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<TRoleClaim>> GetRoleClaimsAsync(string roleId, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(roleId);
            var pagedList = new PagedList<TRoleClaim>();
            var claims = await DbContext.Set<TRoleClaim>().Where(x => x.RoleId.Equals(id))
                .PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await DbContext.Set<TRoleClaim>().Where(x => x.RoleId.Equals(id)).CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual async Task<PagedList<TRoleClaim>> GetUserRoleClaimsAsync(string userId, string claimSearchText, int page = 1, int pageSize = 10)
        {
            var id = ConvertKeyFromString(userId);
            Expression<Func<TRoleClaim, bool>> searchCondition = x => x.ClaimType.Contains(claimSearchText);
            var claimsQ = DbContext.Set<TUserRole>().Where(x => x.UserId.Equals(id))
                .Join(DbContext.Set<TRoleClaim>().WhereIf(!string.IsNullOrEmpty(claimSearchText), searchCondition), ur => ur.RoleId, rc => rc.RoleId, (ur, rc) => rc);

            var claims = await claimsQ.PageBy(x => x.Id, page, pageSize)
                .ToListAsync();

            var pagedList = new PagedList<TRoleClaim>();
            pagedList.Data.AddRange(claims);
            pagedList.TotalCount = await claimsQ.CountAsync();
            pagedList.PageSize = pageSize;

            return pagedList;
        }

        public virtual Task<TUserClaim> GetUserClaimAsync(string userId, int claimId)
        {
            var userIdConverted = ConvertKeyFromString(userId);

            return DbContext.Set<TUserClaim>().Where(x => x.UserId.Equals(userIdConverted) && x.Id == claimId)
                .SingleOrDefaultAsync();
        }



        public virtual Task<TRoleClaim> GetRoleClaimAsync(string roleId, int claimId)
        {
            var roleIdConverted = ConvertKeyFromString(roleId);

            return DbContext.Set<TRoleClaim>().Where(x => x.RoleId.Equals(roleIdConverted) && x.Id == claimId)
                .SingleOrDefaultAsync();
        }



        public virtual async Task<IdentityResult> CreateUserClaimsAsync(TUserClaim claims)
        {
            var user = await UserManager.FindByIdAsync(claims.UserId.ToString());
            return await UserManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public virtual async Task<IdentityResult> UpdateUserClaimsAsync(TUserClaim claims)
        {
            var user = await UserManager.FindByIdAsync(claims.UserId.ToString());
            var userClaim = await DbContext.Set<TUserClaim>().Where(x => x.Id == claims.Id).SingleOrDefaultAsync();

            await UserManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));

            return await UserManager.AddClaimAsync(user, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public virtual async Task<IdentityResult> CreateRoleClaimsAsync(TRoleClaim claims)
        {
            var role = await RoleManager.FindByIdAsync(claims.RoleId.ToString());
            return await RoleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));
        }

        public virtual async Task<IdentityResult> UpdateRoleClaimsAsync(TRoleClaim claims)
        {
            var role = await RoleManager.FindByIdAsync(claims.RoleId.ToString());
            var userClaim = await DbContext.Set<TUserClaim>().Where(x => x.Id == claims.Id).SingleOrDefaultAsync();

            await RoleManager.RemoveClaimAsync(role, new Claim(userClaim.ClaimType, userClaim.ClaimValue));

            return await RoleManager.AddClaimAsync(role, new Claim(claims.ClaimType, claims.ClaimValue));
        }


        public virtual async Task<IdentityResult> DeleteUserClaimAsync(string userId, int claimId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var userClaim = await DbContext.Set<TUserClaim>().Where(x => x.Id == claimId).SingleOrDefaultAsync();

            return await UserManager.RemoveClaimAsync(user, new Claim(userClaim.ClaimType, userClaim.ClaimValue));
        }

        public virtual async Task<IdentityResult> DeleteRoleClaimAsync(string roleId, int claimId)
        {
            var role = await RoleManager.FindByIdAsync(roleId);
            var roleClaim = await DbContext.Set<TRoleClaim>().Where(x => x.Id == claimId).SingleOrDefaultAsync();

            return await RoleManager.RemoveClaimAsync(role, new Claim(roleClaim.ClaimType, roleClaim.ClaimValue));
        }

        public virtual async Task<List<UserLoginInfo>> GetUserProvidersAsync(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var userLoginInfos = await UserManager.GetLoginsAsync(user);

            return userLoginInfos.ToList();
        }

        public virtual Task<TUserLogin> GetUserProviderAsync(string userId, string providerKey)
        {
            var userIdConverted = ConvertKeyFromString(userId);

            return DbContext.Set<TUserLogin>().Where(x => x.UserId.Equals(userIdConverted) && x.ProviderKey == providerKey)
                .SingleOrDefaultAsync();
        }

        public virtual async Task<IdentityResult> DeleteUserProvidersAsync(string userId, string providerKey, string loginProvider)
        {
            var userIdConverted = ConvertKeyFromString(userId);

            var user = await UserManager.FindByIdAsync(userId);
            var login = await DbContext.Set<TUserLogin>().Where(x => x.UserId.Equals(userIdConverted) && x.ProviderKey == providerKey && x.LoginProvider == loginProvider).SingleOrDefaultAsync();
            return await UserManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
        }

        public virtual async Task<IdentityResult> UserChangePasswordAsync(string userId, string password)
        {
            var user = await UserManager.FindByIdAsync(userId);
            var token = await UserManager.GeneratePasswordResetTokenAsync(user);

            return await UserManager.ResetPasswordAsync(user, token, password);
        }

        public virtual async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var userIdentity = await UserManager.FindByIdAsync(userId);

            return await UserManager.DeleteAsync(userIdentity);
        }

        protected virtual async Task<int> AutoSaveChangesAsync()
        {
            return AutoSaveChanges ? await DbContext.SaveChangesAsync() : (int)SavedStatus.WillBeSavedExplicitly;
        }

        public virtual async Task<int> SaveAllChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }
    }
}