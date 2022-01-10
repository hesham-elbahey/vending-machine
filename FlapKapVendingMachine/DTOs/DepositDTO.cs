using System;
using System.ComponentModel.DataAnnotations;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.Domain.Models.Enums;

namespace FlapKapVendingMachine.DTOs
{
    public class DepositDTO
    {
        [Coin]
        public CoinDenomination Value { get; set; }

        [Range(1, 10000, ErrorMessage = "Can only enter from 1 to 10000 coins from the same denomination")]
        public int Quantity { get; set; }
    }
}
