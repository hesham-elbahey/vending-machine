using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace FlapKapVendingMachine.Domain.Models
{
    public class ApplicationUser: IdentityUser
    {
        [DefaultValue(0)]
        public long Deposit { get; set; }

        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
