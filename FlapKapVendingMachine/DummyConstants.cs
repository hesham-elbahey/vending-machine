using System;
namespace FlapKapVendingMachine
{
    public class DummyConstants
    {
        public const string ConnectionString = @"Server=localhost,1433;
        Database=VendingMachine;
        User Id=machineUser;
        Password=Admin#123;
        Integrated Security=False;
        MultipleActiveResultSets=True";

        public const string DatabaseName = "VendingMachine";

        public const string JWTSecret = "2c70e12b7a0646f92279f427c7b38e7334d8e5389cff167a1dc30e73f826b683"; //sha256 hashing of "key"
        public const string JWTIssuer = "Issuer";
        public const string JWTAudience = "Audience";
        public const int JWTAccessTokenExpiration = 5;
        public const int JWTRefreshTokenExpiration = 30;
    }
}
