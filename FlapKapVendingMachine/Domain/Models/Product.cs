using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FlapKapVendingMachine.Attributes;
using Microsoft.EntityFrameworkCore;

namespace FlapKapVendingMachine.Domain.Models
{
    [Index(nameof(ProductName), IsUnique = true)]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Seller))]
        public string SellerId { get; set; }

        [Required]
        public virtual Seller Seller { get; set; }

        [Required]
        [ProductNameUnique]
        public string ProductName { get; set; }

        [Required]
        [Range(5, int.MaxValue, ErrorMessage = "The minimum cost of a product is 5 cents.")]
        public int Cost { get; set; }

        public List<Stock> Stocks { get; set; }
    }
}
