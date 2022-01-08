using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlapKapVendingMachine.Domain.Models
{
    public class StockOrder
    {
        [ForeignKey(nameof(Stock))]
        public int StockId { get; set; }

        [Required]
        public virtual Stock Stock { get; set; }

        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }

        [Required]
        public virtual Order Order { get; set; }
    }
}
