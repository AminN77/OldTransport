using BusinessLogic.Abstractions;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;
using ViewModels;

namespace MvcWebApi.Providers
{
    public class TokenStoreService : ITokenStoreService
    {
        private readonly IOptionsSnapshot<BearerTokensOptions> _configuration;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly ISecurityProvider _securityProvider;

        private readonly IBusinessLogicUserTokenManager _businessLogicTokenManager;

        public TokenStoreService(
            ISecurityProvider securityProvider,
            IOptionsSnapshot<BearerTokensOptions> configuration,
            ITokenFactoryService tokenFactoryService,
            IBusinessLogicUserTokenManager businessLogicTokenManager)
        {
            _securityProvider = securityProvider;
            _configuration = configuration;
            _tokenFactoryService = tokenFactoryService;
            _businessLogicTokenManager = businessLogicTokenManager;
        }

        public async Task AddUserTokenAsync(AddUserTokenViewModel userToken)
        {
            if (!_configuration.Value.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(userToken.UserId);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(userToken.RefreshTokenIdHashSource);
            await _businessLogicTokenManager.AddUserTokenAsync(userToken,1);
        }

        public async Task AddUserTokenAsync(UserSignInViewModel user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial)
        {
            var now = DateTimeOffset.UtcNow;
            var tok = new AddUserTokenViewModel
            {
                UserId = user.Id,
                RefreshTokenIdHash = refreshTokenSerial,
                RefreshTokenIdHashSource = refreshTokenSourceSerial,
                AccessTokenHash = accessToken,
                RefreshTokenExpiresDateTime = now.AddMinutes(_configuration.Value.RefreshTokenExpirationMinutes),
                AccessTokenExpiresDateTime = now.AddMinutes(_configuration.Value.AccessTokenExpirationMinutes)
            };
            if (!_configuration.Value.AllowMultipleLoginsFromTheSameUser)
            {
                await InvalidateUserTokensAsync(tok.UserId);
            }
            await DeleteTokensWithSameRefreshTokenSourceAsync(tok.RefreshTokenIdHashSource);
            await _businessLogicTokenManager.AddUserTokenAsync(tok, 1);
        }

        public async Task DeleteExpiredTokensAsync()
        {
            await _businessLogicTokenManager.DeleteExpiredTokensAsync();
        }

        public async Task DeleteTokenAsync(string refreshTokenValue)
        {
            await _businessLogicTokenManager.DeleteTokenAsync(refreshTokenValue);
        }

        private async Task DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {
            if (string.IsNullOrWhiteSpace(refreshTokenIdHashSource))
            {
                return;
            }
            await _businessLogicTokenManager.DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
        }

        public async Task RevokeUserBearerTokensAsync(string userIdValue, string refreshTokenValue)
        {
            if (!string.IsNullOrWhiteSpace(userIdValue) && int.TryParse(userIdValue, out var userId))
            {
                if (_configuration.Value.AllowSignoutAllUserActiveClients)
                {
                    await InvalidateUserTokensAsync(userId);
                }
            }

            if (!string.IsNullOrWhiteSpace(refreshTokenValue))
            {
                var refreshTokenSerial = _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue);
                if (!string.IsNullOrWhiteSpace(refreshTokenSerial))
                {
                    var refreshTokenIdHashSource = await _securityProvider.Sha256HashAsync(refreshTokenSerial);
                    await DeleteTokensWithSameRefreshTokenSourceAsync(refreshTokenIdHashSource);
                }
            }

            await DeleteExpiredTokensAsync();
        }

        public async Task<ListUserTokenViewModel> FindTokenAsync(string refreshTokenValue)
        {
            var result = await _businessLogicTokenManager.FindTokenAsync(refreshTokenValue);
            return result.Result;
        }

        public async Task<bool> InvalidateUserTokensAsync(int userId)
        {
            var result = await _businessLogicTokenManager.InvalidateUserTokensAsync(userId);
            return result.Succeeded;
        }

        public async Task<bool> IsValidTokenAsync(string accessToken, int userId)
        {
            var result = await _businessLogicTokenManager.IsValidTokenAsync(accessToken, userId);
            return result.Succeeded;
        }
    }
}
