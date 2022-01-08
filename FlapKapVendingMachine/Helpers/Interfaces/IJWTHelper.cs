using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlapKapVendingMachine.Helpers.Interfaces
{
    public interface IJWTHelper
    {
        public string BuildToken(List<Claim> claims);

        public Task<string> BuildRefreshTokenAsync(string userId);

        public Task<bool> RevokeRefreshTokenAsync(string userId, string token);

        public Task RevokeAllRefreshTokensAsync(string userId);

        public ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime = false);
    }
}
