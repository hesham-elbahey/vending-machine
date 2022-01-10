using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.Domain.Models.Enums;
using FlapKapVendingMachine.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Extensions
{
    public static  class DbSetProductExtensions
    {
        public static async Task<bool> CheckUniqueProductNameAsync(this DbSet<Product> products, string productName)
        {
            return !(await products.AnyAsync(p => p.ProductName == productName));
        }

        public static List<ProductDTO> GetProducts(this DbSet<Stock> stocks, string sellerId = null)
        {
            Expression<Func<Stock, bool>> expression = s => true;

            if (sellerId != null)
                expression = s => s.Product.SellerId == sellerId;

            var stocksWithOrdersAndProducts = stocks
                .Include(s => s.Product)
                .Include(s => s.StockOrders)
                .ThenInclude(so => so.Order)
                .Where(expression);
            var result = stocksWithOrdersAndProducts.AsEnumerable().Select(s => {
                return ToProductDTO(out long realAmount, s);
            });
            return result.ToList();
        }

        public static ProductDTO GetProduct(this DbSet<Stock> stocks, int productId)
        {
            var stockProductWithOrder = stocks
                .Include(s => s.Product)
                .Include(s => s.StockOrders)
                .ThenInclude(so => so.Order)
                .FirstOrDefault(s => s.ProductId == productId);
            if (stockProductWithOrder == null)
                return null;

            return ToProductDTO(out long realAmount, stockProductWithOrder);
        }

        private static ProductDTO ToProductDTO(out long realAmount, Stock stockProductWithOrder)
        {
            realAmount = stockProductWithOrder.AmountAvailable
                            - stockProductWithOrder.StockOrders.Sum(so => so.Order.Status != OrderStatus.Canceled ? so.Quantity : 0);
            return new ProductDTO
            {
                AmountAvailable = realAmount,
                Cost = stockProductWithOrder.Product.Cost,
                Id = stockProductWithOrder.ProductId,
                ProductName = stockProductWithOrder.Product.ProductName,
                SellerId = stockProductWithOrder.Product.SellerId
            };
        }
    }
}
