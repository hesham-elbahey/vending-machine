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

        public const string JWTSecret = "Secret";
        public const string JWTIssuer = "Issuer";
        public const string JWTAudience = "Audience";
        public const int JWTAccessTokenExpiration = 5;
        public const int JWTRefreshTokenExpiration = 30;
    }
}
