using System;
using System.Collections.Generic;

namespace FlapKapVendingMachine.Domain.Models
{
    public class Seller: ApplicationUser
    {
        public List<Product> Products { get; set; }
    }
}
