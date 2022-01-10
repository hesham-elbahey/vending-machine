using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.Constants;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.DTOs;
using FlapKapVendingMachine.DTOs.Enums;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // PATCH: api/Users/{id}
        [HttpPatch("{id}")]
        [Authorize(Policy = PolicyNames.SameUser)]
        [ModelValidation]
        public async Task<IActionResult> Patch(UpdateUserDTO updateUserDTO, string id)
        {
            if (updateUserDTO.Id != id)
                return BadRequest();
            var user = await _context.Users.FindAsync(id);
            user.UserName = updateUserDTO.UserName;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        [Authorize(Policy = PolicyNames.SameUser)]
        public async Task<ActionResult<UserDTO>> Get(string id)
        {
            var user = await _context.Users.FindAsync(id);
            UserDTO userDTO = new()
            {
                Deposit = user.Deposit,
                Id = user.Id,
                Type = user is Buyer ? UserType.Buyer : UserType.Seller,
                UserName = user.UserName
            };
            return userDTO;
        }

        // PATCH: api/Users/{id}
        [HttpPatch("{id}/changepassword")]
        [Authorize(Policy = PolicyNames.SameUser)]
        [ModelValidation]
        public async Task<IActionResult> ResetPassword(PasswordDTO passwordDTO, string id)
        {
            var user = await _context.Users.FindAsync(id);
            var result = await _userManager.ChangePasswordAsync(user, passwordDTO.OldPassword, passwordDTO.NewPassword);
            if (result.Succeeded)
                return NoContent();
            return Unauthorized(new { Message = "Current password is incorrect" });
        }
    }
}
