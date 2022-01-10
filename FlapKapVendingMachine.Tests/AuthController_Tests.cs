using System;
using FlapKapVendingMachine.Controllers;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.DTOs;
using FlapKapVendingMachine.Helpers;
using FlapKapVendingMachine.Helpers.Interfaces;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace FlapKapVendingMachine.Tests
{
    public class AuthController_Tests
    {
        [Fact]
        public async void Register()
        {
            //Arrange
            var options = new DbContextOptionsBuilder<AppDbContext>()
               .UseInMemoryDatabase(databaseName: DummyConstants.DatabaseName)
               .Options;
            using var context = new AppDbContext(options);
            var roleStore = new RoleStore<IdentityRole>(context);
            var _roleManager = IdentityHelper.GetRoleManager(roleStore);
            var userStore = new UserStore<ApplicationUser>(context);
            var _userManager = IdentityHelper.GetUserManager(userStore);
            var _jWTHelper = new JWTHelper(context, _userManager);
            var _signInManager = IdentityHelper.GetSignInManager(_userManager);
            var controller = new AuthController(_userManager, _roleManager, _jWTHelper, _signInManager);
            //Act
            RegisterDTO allowedRegister1 = new() { UserName = "allowed only", Password = "password" };
            RegisterDTO allowedRegister2 = new() { UserName = "another allowed", Password = "password" };
            RegisterDTO allowedRegister3 = new() { UserName = "another allowed", Password = "password" };
            //Assert
            await controller.Register(allowedRegister1);
            await controller.Register(allowedRegister2);
            await controller.Register(allowedRegister3);
            var count = await _userManager.Users.CountAsync();
            Assert.Equal(3, count);
        }
    }
}
