using System;
using System.Threading.Tasks;
using FlapKapVendingMachine.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Extensions
{
    public static class DbSetUserExtensions
    {
        public static async Task<bool> CheckUniqueUserNameAsync(this DbSet<ApplicationUser> users, string userName)
        {
            return !(await users.AnyAsync(u => u.UserName == userName));
        }
    }
}
