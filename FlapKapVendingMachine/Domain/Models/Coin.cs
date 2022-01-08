using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlapKapVendingMachine.Domain.Models.Enums;

namespace FlapKapVendingMachine.Domain.Models
{
    public class Coin
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public CoinDenomination Value { get; set; }

        [DefaultValue(1)]
        [Range(1, 10000, ErrorMessage = "Can only enter from 1 to 10000 coins from the same denomination")]
        public int Quantity { get; set; } = 1;

        [ForeignKey(nameof(VendingMachine))]
        public int VendingMachineId { get; set; }

        [Required]
        public virtual VendingMachine VendingMachine { get; set; }
    }
}
