using System;
using FlapKapVendingMachine.DTOs.Enums;

namespace FlapKapVendingMachine.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }

        public string UserName { get; set; }

        public long Deposit { get; set; }

        public UserType Type { get; set; }
    }

    public class TokenResponse
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }

    public class LoginResponse : TokenResponse
    {
        public UserDTO User { get; set; }
    }
}
