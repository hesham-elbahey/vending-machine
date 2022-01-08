using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlapKapVendingMachine.Domain.Models.Enums;

namespace FlapKapVendingMachine.Domain.Models
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        [DefaultValue(OrderStatus.Pending)]
        public OrderStatus Status { get; set; }

        [ForeignKey(nameof(Buyer))]
        public string BuyerId { get; set; }

        [Required]
        public virtual Buyer Buyer { get; set; }

        public List<StockOrder> StockOrders { get; set; }
    }
}
