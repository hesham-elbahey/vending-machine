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
using Microsoft.IdentityModel.Tokens;

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
        /// Register new users
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
            user.UserName = registerDTO.UserName;
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
        /// Login users
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

        /// <summary>
        /// Using the refresh token to stay logged in for as long as possible
        /// </summary>
        /// <param name="tokenDTO">A DTO containing an access and a refresh token</param>
        /// <returns>The newly created access and refresh tokens for further use</returns>
        [Route("refreshtoken")]
        [HttpPost]
        [ModelValidation]
        public async Task<ActionResult<TokenResponse>> RefreshToken(TokenDTO tokenDTO)
        {
            ClaimsPrincipal principal;
            try
            {
                principal = _jWTHelper.GetPrincipalFromToken(tokenDTO.AccessToken);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { ex.Message });
            }
            if (principal == null)
                return Unauthorized(new { Message = "Invalid access token." });
            string userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (await _jWTHelper.RevokeRefreshTokenAsync(userId, tokenDTO.RefreshToken))
                {
                    string refreshToken = await _jWTHelper.BuildRefreshTokenAsync(userId);
                    string accessToken = _jWTHelper.BuildToken(principal.Claims.ToList());
                    return new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
                }
                return Unauthorized(new { Message = "Invalid refresh token." });
            }
            catch (SecurityTokenExpiredException ex)
            {
                return Unauthorized(new { ex.Message });
            }
        }

    }
}