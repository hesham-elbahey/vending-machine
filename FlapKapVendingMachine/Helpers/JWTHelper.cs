using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FlapKapVendingMachine.Domain.Models;
using FlapKapVendingMachine.DTOs;
using FlapKapVendingMachine.DTOs.Enums;
using FlapKapVendingMachine.Helpers.Interfaces;
using FlapKapVendingMachine.Persistence.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace FlapKapVendingMachine.Helpers
{
    public class JWTHelper: IJWTHelper
    {
        private readonly AppDbContext _context;

        private readonly UserManager<ApplicationUser> _userManager;

        public JWTHelper(AppDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string BuildToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DummyConstants.JWTSecret));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: DummyConstants.JWTIssuer,
                    audience: DummyConstants.JWTAudience,
                    notBefore: DateTime.UtcNow,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(DummyConstants.JWTAccessTokenExpiration),
                    signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> BuildRefreshTokenAsync(string userId)
        {
            var randomNumber = new byte[32];
            using var randomNumberGenerator = RandomNumberGenerator.Create();
            randomNumberGenerator.GetBytes(randomNumber);
            string token = Convert.ToBase64String(randomNumber);
            RefreshToken refreshToken = new()
            {
                UserId = userId,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(DummyConstants.JWTRefreshTokenExpiration),
                Token = token
            };
            _context.RefreshTokens.Add(refreshToken);
            await _context.SaveChangesAsync();
            return token;
        }

        public ClaimsPrincipal GetPrincipalFromToken(string token, bool validateLifetime = false)
        {
            JwtSecurityTokenHandler tokenValidator = new();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(DummyConstants.JWTSecret));

            var parameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                ValidateLifetime = validateLifetime,
            };
            try
            {
                var principal = tokenValidator.ValidateToken(token, parameters, out var securityToken);


                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                throw;
            }

        }

        public async Task<bool> RevokeRefreshTokenAsync(string userId, string token)
        {
            RefreshToken oldRefreshToken = _context.RefreshTokens.FirstOrDefault(r => r.UserId == userId && r.Token == token);
            if (oldRefreshToken == null)
                return false;
            if (oldRefreshToken.ExpiresAt < DateTime.UtcNow)
                throw new SecurityTokenExpiredException("Refresh token expired.");
            _context.RefreshTokens.Remove(oldRefreshToken);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task RevokeAllRefreshTokensAsync(string userId)
        {
            var refreshTokens = _context.RefreshTokens.Where(r => r.UserId == userId);
            _context.RemoveRange(refreshTokens);
            await _context.SaveChangesAsync();
        }

        private async Task<TokenResponse> BuildTokensAsync(ApplicationUser user)
        {
            var userRoles = (await _userManager.GetRolesAsync(user)).Select(c => new Claim(ClaimTypes.Role, c));
            var claims = BuildClaims(user);
            claims.AddRange(userRoles);
            var accessToken = BuildToken(claims);
            var refreshToken = await BuildRefreshTokenAsync(user.Id);
            return new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
        }

        private static List<Claim> BuildClaims(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            return claims;
        }

        public async Task<LoginResponse> GetLoginResponseAsync(ApplicationUser user)
        {
            UserDTO userDTO = new();
            userDTO.Id = user.Id;
            userDTO.UserName = user.UserName;
            userDTO.Type = user is Buyer ? UserType.Buyer : UserType.Seller;
            userDTO.Deposit = user.Deposit;
            var tokenResponse = await BuildTokensAsync(user);
            var loginResponse = new LoginResponse
            {
                AccessToken = tokenResponse.AccessToken,
                RefreshToken = tokenResponse.RefreshToken,
                User = userDTO
            };
            return loginResponse;
        }
    }
}
