using MvcWebApi.Models;
using System.Threading.Tasks;
using ViewModels;

namespace MvcWebApi.Providers
{
    public interface ITokenFactoryService
    {
        Task<JwtTokensData> CreateJwtTokensAsync(UserSignInViewModel user);
        string GetRefreshTokenSerial(string refreshTokenValue);
    }
}
