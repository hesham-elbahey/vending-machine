using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlapKapVendingMachine.Domain.Models
{
    [Table(nameof(VendingMachine) + "s")]
    public class VendingMachine
    {
        [Key]
        public int Id { get; set; }

        public List<Coin> Coins { get; set; }

        public List<Stock> Stocks { get; set; }
    }
}
