using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Http;

namespace FlapKapVendingMachine.DTOs
{
    public class ProductDTO: IValidatableObject
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string SellerId { get; set; }

        [Required]
        [Range(1, 10000, ErrorMessage = "Can only have between 1 to 10000 items in stock.")]
        public long AmountAvailable { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        [Range(5, int.MaxValue, ErrorMessage = "The minimum cost of a product is 5 cents.")]
        public int Cost { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            AppDbContext context = validationContext
                .GetService(typeof(AppDbContext)) as AppDbContext;
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            if (int.TryParse((string)httpContextAccessor.HttpContext.Request.RouteValues["productId"], out int productId))
            {
                if (productId == Id)
                {
                    yield return ValidationResult.Success;
                    yield break;
                }
            }
            
            if (context.Products.Any(p => p.ProductName == ProductName))
                yield return new ValidationResult("Product name already exists.", new List<string>() { nameof(ProductName) });
            yield return ValidationResult.Success;
        }
    }
}
