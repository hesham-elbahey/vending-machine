using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FlapKapVendingMachine.Domain.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public long AmountAvailable { get; set; }

        [ForeignKey(nameof(VendingMachine))]
        public int VendingMachineId { get; set; }

        public virtual VendingMachine VendingMachine { get; set; }

        public List<StockOrder> StockOrders { get; set; }


    }
}
