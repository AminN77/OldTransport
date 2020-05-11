using System.Threading.Tasks;
using ViewModels;

namespace MvcWebApi.Providers
{
    public interface ITokenStoreService
    {
        Task AddUserTokenAsync(AddUserTokenViewModel userToken);
        Task AddUserTokenAsync(UserSignInViewModel user, string refreshTokenSerial, string accessToken, string refreshTokenSourceSerial);
        //Task AddUserTokenAsync(UserSignInViewModel user, string refreshToken, string accessToken,DateTimeOffset refreshTokenExpiresDateTime, DateTimeOffset accessTokenExpiresDateTime);
        Task<bool> IsValidTokenAsync(string accessToken, int userId);
        Task DeleteExpiredTokensAsync();
        Task<ListUserTokenViewModel> FindTokenAsync(string refreshToken);
        Task DeleteTokenAsync(string refreshToken);
        Task<bool> InvalidateUserTokensAsync(int userId);
    }
}
