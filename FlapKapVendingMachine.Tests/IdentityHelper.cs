using System;
using FlapKapVendingMachine.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace FlapKapVendingMachine.Tests
{
    public static class IdentityHelper
    {
        public static RoleManager<TIdentityRole> GetRoleManager<TIdentityRole>(IRoleStore<TIdentityRole> roleStore)
            where TIdentityRole : IdentityRole
        {
            return new RoleManager<TIdentityRole>(
                roleStore,
                new IRoleValidator<TIdentityRole>[0],
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<ILogger<RoleManager<TIdentityRole>>>().Object
                );
        }

        public static UserManager<TIdentityUser> GetUserManager<TIdentityUser>(IUserStore<TIdentityUser> userStore)
            where TIdentityUser : IdentityUser
        {
            return new UserManager<TIdentityUser>(
                userStore,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<TIdentityUser>>().Object,
                new IUserValidator<TIdentityUser>[0],
                new IPasswordValidator<TIdentityUser>[0],
                new UpperInvariantLookupNormalizer(),
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<TIdentityUser>>>().Object
                );
        }

        public static SignInManager<TIdentityUser> GetSignInManager<TIdentityUser>(UserManager<TIdentityUser> userManager)
            where TIdentityUser : IdentityUser
        {
            return new SignInManager<TIdentityUser>(userManager, new Mock<IHttpContextAccessor>().Object,
                 new Mock<IUserClaimsPrincipalFactory<TIdentityUser>>().Object,
                 new Mock<IOptions<IdentityOptions>>().Object,
                 new Mock<ILogger<SignInManager<TIdentityUser>>>().Object,
                 new Mock<IAuthenticationSchemeProvider>().Object, new Mock<IUserConfirmation<TIdentityUser>>().Object);
        }

    }
}
