using System;
using System.ComponentModel.DataAnnotations;

namespace FlapKapVendingMachine.DTOs
{
    public class TokenDTO
    {
        [Required]
        public string AccessToken { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
