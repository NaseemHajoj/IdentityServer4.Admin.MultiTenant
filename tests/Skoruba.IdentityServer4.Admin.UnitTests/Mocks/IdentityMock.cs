using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Entities.Identity;
using Skoruba.IdentityServer4.Admin.EntityFramework.Shared.Identity;

namespace Skoruba.IdentityServer4.Admin.UnitTests.Mocks
{
    public static class IdentityMock
    {
        public static Faker<UserIdentityUserLogin> GetUserProvidersFaker(string key, string loginProvider, string userId)
        {
            var userProvidersFaker = new Faker<UserIdentityUserLogin>()
                .RuleFor(o => o.LoginProvider, f => loginProvider)
                .RuleFor(o => o.ProviderKey, f => key)
                .RuleFor(o => o.ProviderDisplayName, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.UserId, userId);

            return userProvidersFaker;
        }

        public static UserIdentityUserLogin GenerateRandomUserProviders(string key, string loginProvider, string userId)
        {
            var provider = GetUserProvidersFaker(key, loginProvider, userId).Generate();

            return provider;
        }

        // NOTE: These mocks are from - https://github.com/aspnet/Identity/blob/master/test/Shared/MockHelpers.cs
        public static ApplicationUserManager<TUser, TKey> TestUserManager<TUser, TKey>(ApplicationUserStore<TUser, TKey> store = null) 
            where TUser : ApplicationUser<TKey>, new()
            where TKey : IEquatable<TKey>
        {
            store = store ?? new Mock<ApplicationUserStore<TUser, TKey>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions { Lockout = { AllowedForNewUsers = false } };
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var validator = new Mock<IUserValidator<TUser>>();
            userValidators.Add(validator.Object);
            var pwdValidators = new List<PasswordValidator<TUser>> { new PasswordValidator<TUser>() };
            var userManager = new ApplicationUserManager<TUser, TKey>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null,
                new Mock<ILogger<UserManager<TUser>>>().Object);
            validator.Setup(v => v.ValidateAsync(userManager, It.IsAny<TUser>()))
                .Returns(Task.FromResult(IdentityResult.Success)).Verifiable();
            return userManager;
        }

        public static RoleManager<TRole> TestRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class
        {
            store = store ?? new Mock<IRoleStore<TRole>>().Object;
            var roles = new List<IRoleValidator<TRole>> { new RoleValidator<TRole>() };
            return new RoleManager<TRole>(store, roles,
                new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(),
                null);
        }
    }
}