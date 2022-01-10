using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlapKapVendingMachine.Persistence.Contexts;

namespace FlapKapVendingMachine.Attributes
{
   
    public class UserNameUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AppDbContext context = validationContext
                .GetService(typeof(AppDbContext)) as AppDbContext;
            if (context.Users.Any(u => u.UserName == value as string))
                return new ValidationResult("UserName already exists.");
            return ValidationResult.Success;
        }
    }
    
}
