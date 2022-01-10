using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlapKapVendingMachine.DTOs.Enums;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Http;

namespace FlapKapVendingMachine.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public long Deposit { get; set; }

        public UserType Type { get; set; }

    }

    public class UpdateUserDTO : IValidatableObject
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string UserName { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            AppDbContext context = validationContext
                .GetService(typeof(AppDbContext)) as AppDbContext;
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            string userId = (string)httpContextAccessor.HttpContext.Request.RouteValues["id"];
            
            if (userId == Id)
            {
                yield return ValidationResult.Success;
                yield break;
            }
            

            if (context.Users.Any(u => u.UserName == UserName))
                yield return new ValidationResult("Username already exists.", new List<string>() { nameof(UserName) });
            yield return ValidationResult.Success;
        }
    }

    public class PasswordDTO
    {
        [Required]
        public string OldPassword { get; set; }

        [Required]
        public string NewPassword { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class LoginResponse : TokenResponse
    {
        public UserDTO User { get; set; }
    }
}
