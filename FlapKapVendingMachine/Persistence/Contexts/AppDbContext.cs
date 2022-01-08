using System;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.Domain.Models.Enums;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Persistence.Contexts
{
    public class AppDbContext: IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<StockOrder>().HasKey(so => new { so.OrderId, so.StockId });
            builder.Entity<Stock>().HasIndex(s => new { s.ProductId, s.VendingMachineId }).IsUnique();
            builder.Entity<Coin>().Property(c => c.Quantity).HasDefaultValue(1);
            builder.Entity<Order>().Property(o => o.Status).HasDefaultValue(OrderStatus.Pending);
            builder.Entity<Stock>().HasMany(s => s.StockOrders).WithOne(so => so.Stock).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<VendingMachine>().HasData(new VendingMachine { Id = 1 });
            builder.Entity<Coin>().HasData(new Coin { Id = 1, Value = CoinDenomination.Five, VendingMachineId = 1, Quantity = 100 },
                new Coin { Id = 2, Value = CoinDenomination.Ten, VendingMachineId = 1, Quantity = 100 },
                new Coin { Id = 3, Value = CoinDenomination.Twenty, VendingMachineId = 1, Quantity = 100 },
                new Coin { Id = 4, Value = CoinDenomination.Fifty, VendingMachineId = 1, Quantity = 100 },
                new Coin { Id = 5, Value = CoinDenomination.Hundred, VendingMachineId = 1, Quantity = 100 });
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<VendingMachine> VendingMachines { get; set; }

        public DbSet<Coin> Coins { get; set; }

        public DbSet<Order> Orders { get; set; }

        public DbSet<Stock> Stocks { get; set; }

        public DbSet<StockOrder> StockOrders { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
