using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.Constants;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.DTOs;
using FlapKapVendingMachine.DTOs.Enums;
using FlapKapVendingMachine.Extensions;
using FlapKapVendingMachine.Helpers.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlapKapVendingMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly IJWTHelper _jWTHelper;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IJWTHelper jWTHelper,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jWTHelper = jWTHelper;
            _signInManager = signInManager;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="registerDTO">A DTO containing the username, password, and type of a user</param>
        /// <returns>An ActionResult indicating whether the user was created or not</returns>
        [HttpPost("register")]
        [ModelValidation]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            ApplicationUser user;
            string roleName;
            if (registerDTO.Type == UserType.Buyer)
            {
                user = new Buyer();
                roleName = RoleNames.Buyer;
            }
            else
            {
                user = new Seller();
                roleName = RoleNames.Seller;
            }
            await _userManager.CreateAsync(user, registerDTO.Password);
            if (!(await _roleManager.RoleExistsAsync(roleName)))
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = roleName, NormalizedName = roleName });
            }
            await _userManager.AddToRoleAsync(user, roleName);
            UserDTO userDTO = new() { Id = user.Id, UserName = user.UserName, Type = registerDTO.Type };

            return Created($"/api/users/{user.Id}", userDTO);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loginDTO">A DTO containing the username and password of a user</param>
        /// <returns>An ActionResult containing an access token and a refresh token upon success</returns>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginDTO loginDTO)
        {
            var unsuccessful = new { Message = "Username and Password don't match." };
            var user = _userManager.Users.FirstOrDefault(u => u.UserName ==  loginDTO.UserName);
            if (user == null)
                return Unauthorized(unsuccessful);
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, true);
            if (result.Succeeded)
                return Ok(await _jWTHelper.GetLoginResponseAsync(user));
            return Unauthorized(unsuccessful);
        }
        
    }
}