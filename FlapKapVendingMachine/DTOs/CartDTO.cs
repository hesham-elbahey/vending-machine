using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using FlapKapVendingMachine.Extensions;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Http;

namespace FlapKapVendingMachine.DTOs
{
    public class CartDTO: IValidatableObject
    {
        public List<ItemDTO> Cart { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            const string ProductId = "ProductId";
            bool yieldBreak = false;
            const string Quantity = "Quantity";
            var quantityList = Cart.Select(i => i.Quantity);
            var productList = Cart.Select(i => i.ProductId);
            
            if (quantityList.Max() > 1000 || quantityList.Min() < 1)
            {
                yieldBreak = true;
                yield return new ValidationResult("Quantities must be within 1 to 1000 range", new List<string>() { nameof(Quantity) });
            }
            if (productList.Distinct().Count() != productList.Count())
            {
                yieldBreak = true;
                yield return new ValidationResult("Product Ids must be unique", new List<string>() { nameof(ProductId) });
            }
            AppDbContext context = validationContext
               .GetService(typeof(AppDbContext)) as AppDbContext;
            var allProductsList = context.Stocks.Select(s => s.ProductId).ToList();
            if (!productList.All(p => allProductsList.Contains(p)))
            {
                yieldBreak = true;
                yield return new ValidationResult("Not all Product Ids are correct", new List<string>() { nameof(ProductId) });
            }
            if (yieldBreak)
                yield break;
            var cartDictionary = Cart.ToDictionary(k => k.ProductId, v => v.Quantity);
            var httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            string buyerId = (string)httpContextAccessor.HttpContext.Request.RouteValues["id"];
            var buyer = context.Users.Find(buyerId);
            var actualStocks = context.Stocks.GetProducts().Where(a => productList.Contains(a.Id));
            if (actualStocks.Any(a => a.AmountAvailable < cartDictionary[a.Id]))
            {
                yield return new ValidationResult("Products are out of stock");
            }
            long totalValue = context.Stocks.Select(s => s.Product).Where(p => productList.Contains(p.Id)).ToList().Sum(p => p.Cost * cartDictionary[p.Id]);
            if (totalValue > buyer.Deposit)
                yield return new ValidationResult("Insufficient funds");
        }
    }

    public class ItemDTO
    {
        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
