using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MvcWebApi.Models;
using ViewModels;

namespace MvcWebApi.Providers
{
    public class TokenFactoryService : ITokenFactoryService
    {
        private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
        private readonly IBusinessLogicRoleManager _businessLogicRoleManager;
        private readonly ILogger<TokenFactoryService> _logger;
        private readonly ISecurityService _securityService;

        public TokenFactoryService(
            IOptionsSnapshot<BearerTokensOptions> configuration,
            ILogger<TokenFactoryService> logger,
            ISecurityService securityService,
            IBusinessLogicRoleManager businessLogicRoleManager)
        {
            _securityService = securityService;
            _configuration = configuration;
            _logger = logger;
            _businessLogicRoleManager = businessLogicRoleManager;
        }

        public async Task<JwtTokensData> CreateJwtTokensAsync(UserSignInViewModel user)
        {
            var (accessToken, claims) = await CreateAccessTokenAsync(user);
            var (refreshTokenValue, refreshTokenSerial) = CreateRefreshToken();
            return new JwtTokensData
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenValue,
                RefreshTokenSerial = refreshTokenSerial,
                Claims = claims
            };
        }

        private (string RefreshTokenValue, string RefreshTokenSerial) CreateRefreshToken()
        {
            var refreshTokenSerial = _securityService.CreateCryptographicallySecureGuid().ToString().Replace("-", "");

            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                // Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                // for invalidation
                new Claim(ClaimTypes.SerialNumber, refreshTokenSerial, ClaimValueTypes.String, _configuration.Value.Issuer)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                signingCredentials: credentials);
            var refreshTokenValue = new JwtSecurityTokenHandler().WriteToken(token);
            return (refreshTokenValue, refreshTokenSerial);
        }

        public string GetRefreshTokenSerial(string refreshTokenValue)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                return null;
            }

            ClaimsPrincipal decodedRefreshTokenPrincipal = null;
            try
            {
                decodedRefreshTokenPrincipal = new JwtSecurityTokenHandler().ValidateToken(
                    refreshTokenValue,
                    new TokenValidationParameters
                    {
                        RequireExpirationTime = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key)),
                        ValidateIssuerSigningKey = true, // verify signature to avoid tampering
                        ValidateLifetime = true, // validate the expiration
                        ClockSkew = TimeSpan.Zero // tolerance for the expiration date
                    },
                    out _
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to validate refreshTokenValue: `{refreshTokenValue}`.");
            }

            return decodedRefreshTokenPrincipal?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
        }

        private async Task<(string AccessToken, IEnumerable<Claim> Claims)> CreateAccessTokenAsync(UserSignInViewModel user)
        {
            var a = _configuration.Value;
            var claims = new List<Claim>
            {
                // Unique Id for all Jwt tokes
                new Claim(JwtRegisteredClaimNames.Jti, _securityService.CreateCryptographicallySecureGuid().ToString(), ClaimValueTypes.String, _configuration.Value.Issuer),
                //// Issuer
                new Claim(JwtRegisteredClaimNames.Iss, _configuration.Value.Issuer, ClaimValueTypes.String, _configuration.Value.Issuer),
                //// Issued at
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64, _configuration.Value.Issuer),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                //// to invalidate the cookie
                new Claim(ClaimTypes.SerialNumber, user.SerialNumber, ClaimValueTypes.String, _configuration.Value.Issuer),
                //// custom data
                new Claim(ClaimTypes.UserData, user.Id.ToString(), ClaimValueTypes.String, _configuration.Value.Issuer)
            };

            // add roles
            var roles = await _businessLogicRoleManager.FindUserRolesAsync(user.Id);
            //var roles = await _rolesService.FindUserRolesAsync(user.Id);
            foreach (var role in roles.Result)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name, ClaimValueTypes.String, _configuration.Value.Issuer));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var now = DateTime.UtcNow;
            var token = new JwtSecurityToken(
                issuer: _configuration.Value.Issuer,
                audience: _configuration.Value.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes),
                signingCredentials: credentials);
            return (new JwtSecurityTokenHandler().WriteToken(token), claims);
        }
    }
}