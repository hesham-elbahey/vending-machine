using System;
using System.ComponentModel.DataAnnotations;
using FlapKapVendingMachine.Attributes;
using FlapKapVendingMachine.DTOs.Enums;

namespace FlapKapVendingMachine.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9_\\s\\-\\.\\@\\+/]+$")]
        [UserNameUnique]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

        public UserType Type { get; set; } = UserType.Buyer;
    }
}
