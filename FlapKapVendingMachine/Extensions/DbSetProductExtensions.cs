using System;
using System.Threading.Tasks;
using FlapKapVendingMachine.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Extensions
{
    public static  class DbSetProductExtensions
    {
        public static async Task<bool> CheckUniqueProductNameAsync(this DbSet<Product> products, string productName)
        {
            return !(await products.AnyAsync(p => p.ProductName == productName));
        }
    }
}
