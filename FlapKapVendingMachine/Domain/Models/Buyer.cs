using System;
using System.Collections.Generic;

namespace FlapKapVendingMachine.Domain.Models
{
    public class Buyer: ApplicationUser
    {
       public List<Order> Orders { get; set; }
    }
}
