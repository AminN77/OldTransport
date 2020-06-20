using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicUserTokenManager:IDisposable
    {
        //Task<IBusinessLogicResult<AddUserTokenViewModel>> AddUserTokenAsync(AddUserTokenViewModel addTokenViewModel, int adderUserId);
        Task<IBusinessLogicResult<AddUserTokenViewModel>> AddUserTokenAsync(AddUserTokenViewModel addTokenViewModel, int adderUserId);
        Task<IBusinessLogicResult> DeleteExpiredTokensAsync();
        Task<IBusinessLogicResult> DeleteTokenAsync(string refreshTokenId);
        Task<IBusinessLogicResult> DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource);
        Task<IBusinessLogicResult<ListUserTokenViewModel>> FindTokenAsync(string refreshTokenId);
        Task<IBusinessLogicResult> InvalidateUserTokensAsync(int userId);
        Task<IBusinessLogicResult> IsValidTokenAsync(string accessToken, int userId);
    }
}
