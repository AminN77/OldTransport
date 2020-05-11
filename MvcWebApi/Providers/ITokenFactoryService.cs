using MvcWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
