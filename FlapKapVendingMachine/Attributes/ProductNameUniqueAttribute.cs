using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlapKapVendingMachine.Persistence.Contexts;

namespace FlapKapVendingMachine.Attributes
{
    public class ProductNameUniqueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            AppDbContext context = validationContext
                .GetService(typeof(AppDbContext)) as AppDbContext;
            if (context.Products.Any(p => p.ProductName == value as string))
                return new ValidationResult("Product name already exists.");
            return ValidationResult.Success;
        }
    }
}
