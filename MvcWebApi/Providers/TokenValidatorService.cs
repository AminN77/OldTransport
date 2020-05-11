using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;

namespace MvcWebApi.Providers
{
    public class TokenValidatorService : ITokenValidatorService
    {
        private readonly IBusinessLogicUserManager _businessLogicUserManager;
        private readonly ITokenStoreService _tokenStoreService;

        public TokenValidatorService(IBusinessLogicUserManager businessLogicUserManager, ITokenStoreService tokenStoreService)
        {
            _businessLogicUserManager = businessLogicUserManager;
            _tokenStoreService = tokenStoreService;
        }

        public async Task ValidateAsync(TokenValidatedContext context)
        {
            var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
            if (claimsIdentity?.Claims == null || !claimsIdentity.Claims.Any())
            {
                context.Fail("This is not our issued token. It has no claims.");
                return;
            }

            var serialNumberClaim = claimsIdentity.FindFirst(ClaimTypes.SerialNumber);
            if (serialNumberClaim == null)
            {
                context.Fail("This is not our issued token. It has no serial.");
                return;
            }

            var userIdString = claimsIdentity.FindFirst(ClaimTypes.UserData).Value;
            if (!int.TryParse(userIdString, out int userId))
            {
                context.Fail("This is not our issued token. It has no user-id.");
                return;
            }

            var user = await _businessLogicUserManager.FindUserAsync(userId).ConfigureAwait(false);
            if (user == null || user.Result.SerialNumber != serialNumberClaim.Value || !user.Result.IsEnabled)
            {
                // user has changed his/her password/roles/stat/IsActive
                context.Fail("This token is expired. Please login again.");
            }

            var accessToken = context.SecurityToken as JwtSecurityToken;
            if (accessToken == null || string.IsNullOrWhiteSpace(accessToken.RawData) ||
                !await _tokenStoreService.IsValidTokenAsync(accessToken.RawData, userId).ConfigureAwait(false))
            {
                context.Fail("This token is not in our database.");
                return;
            }

            await _businessLogicUserManager.UpdateUserLastActivityDateAsync(userId);
        }
    }
}
