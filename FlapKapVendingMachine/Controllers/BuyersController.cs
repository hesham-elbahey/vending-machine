using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.Constants;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.Domain.Models.Enums;
using FlapKapVendingMachine.DTOs;
using FlapKapVendingMachine.DTOs.Enums;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BuyersController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Deposit coins into the vending machine
        /// </summary>
        /// <param name="depositDTOs">A DTO containing different coin denominations with their quantities</param>
        /// <param name="id">The buyer ID</param>
        /// <returns>A list of coin denominations with their updated quantities</returns>
        // POST: api/Buyers/{id}/deposit
        [HttpPost("~/api/buyers/{id}/deposit")]
        [Authorize(Policy = PolicyNames.SameBuyer)]
        [ModelValidation]
        public async Task<ActionResult<List<DepositDTO>>> Deposit(List<DepositDTO> depositDTOs, string id)
        {
            var user = await _context.Users.FindAsync(id);
            var sum = depositDTOs.Sum(d => d.Quantity * ((int)d.Value));
            user.Deposit += sum;
            _context.Entry(user).State = EntityState.Modified;
            Coin coin;
            foreach (var deposit in depositDTOs)
            {
                coin = await _context.Coins.FirstOrDefaultAsync(c => c.Value == deposit.Value);
                coin.Quantity += deposit.Quantity;
                _context.Entry(coin).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            var coinDTOs = _context.Coins.Select(c => new DepositDTO { Quantity = c.Quantity, Value = c.Value }).ToList();
            return coinDTOs;
        }

        /// <summary>
        /// Withdraw all of the current balance from the vending machine
        /// </summary>
        /// <param name="id">The buyer ID</param>
        /// <returns>A list of coin denominations with their updated quantities</returns>
        // POST: api/Buyers/{id}/reset
        [HttpPost("~/api/buyers/{id}/reset")]
        [Authorize(Policy = PolicyNames.SameBuyer)]
        public async Task<ActionResult<List<DepositDTO>>> Reset(string id)
        {
            var user = await _context.Users.FindAsync(id);
            var deposit = user.Deposit;
            var coins = await _context.Coins.OrderByDescending(c => (int)c.Value).ToListAsync();
            foreach (var coin in coins)
            {
                while (((int)coin.Value) <= user.Deposit && coin.Quantity > 0)
                {
                    coin.Quantity--;
                    user.Deposit -= ((int)coin.Value);
                }
                _context.Entry(coin).State = EntityState.Modified;
            }
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            var coinDTOs = coins.Select(c => new DepositDTO { Quantity = c.Quantity, Value = c.Value }).ToList();
            return coinDTOs;
        }

        /// <summary>
        /// Buy products from the vending machine
        /// </summary>
        /// <param name="cartDTO">A DTO containing the list of products with their quantities to be bought</param>
        /// <param name="id">The buyer ID</param>
        /// <returns>The user object</returns>
        // POST: api/Buyers/{id}/buy
        [HttpPost("~/api/buyers/{id}/buy")]
        [Authorize(Policy = PolicyNames.SameBuyer)]
        [ModelValidation]
        public async Task<ActionResult<UserDTO>> Buy(CartDTO cartDTO, string id)
        {
            var cartDictionary = cartDTO.Cart.ToDictionary(k => k.ProductId, v => v.Quantity);
            var productList = cartDTO.Cart.Select(c => c.ProductId);
            var stockList = _context.Stocks.Include(s => s.Product)
                .ThenInclude(p => p.Seller)
                .Where(s => productList.Contains(s.ProductId));
            Seller seller;
            long totalValue = _context.Stocks
                .Select(s => s.Product)
                .Where(p => productList.Contains(p.Id)).ToList().Sum(p => p.Cost * cartDictionary[p.Id]);
            Order order = new() { BuyerId = id, Status = OrderStatus.Completed};
            _context.Add(order);
            await _context.SaveChangesAsync();
            StockOrder stockOrder;
            foreach (var stock in stockList)
            {
                stockOrder = new() { OrderId = order.Id, StockId = stock.Id, Quantity = cartDictionary[stock.ProductId]};
                seller = stock.Product.Seller;
                seller.Deposit += stock.Product.Cost * stockOrder.Quantity;
                _context.StockOrders.Add(stockOrder);
                _context.Entry(seller).State = EntityState.Modified;
            }
            var buyer = await _context.Users.FindAsync(id);
            buyer.Deposit -= totalValue;
            await _context.SaveChangesAsync();
            UserDTO userDTO = new() { Deposit = buyer.Deposit, Id = buyer.Id, Type = UserType.Buyer, UserName = buyer.UserName };
            return userDTO;
        }
    }
}
