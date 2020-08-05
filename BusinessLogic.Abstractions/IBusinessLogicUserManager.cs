using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicUserManager : IDisposable
    {
        //Task<IBusinessLogicResult> ResetPasswordAsync(UserSetPasswordViewModel adminUserSetPasswordViewModel, int reSetterUserId);
        Task<IBusinessLogicResult<AddUserViewModel>> AddUserAsync(EmailViewModel emailViewModel);
        Task<IBusinessLogicResult<ListResultViewModel<ListUserViewModel>>> GetUsersAsync(int getterUserId, int page,
            int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<ListResultViewModel<ListMerchantViewModel>>> GetMerchantsAsync(int getterUserId, int page,
            int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<ListResultViewModel<ListTransporterViewModel>>> GetTransportersAsync(int getterUserId, int page,
            int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<EditUserViewModel>> EditUserAsync(EditUserViewModel editUserViewModel, int editorUserId);
        Task<IBusinessLogicResult> DeleteUserAsync(int? userId, int deleterUserId);
        Task<IBusinessLogicResult<EditUserViewModel>> GetUserForEditAsync(int getterUserId);
        Task<IBusinessLogicResult<DetailUserViewModel>> GetUserDetailsAsync(int? userId, int getterUserId);
        Task<IBusinessLogicResult> ChangePasswordAsync(UserChangePasswordViewModel userChangePasswordViewModel, int changerUserId);
        Task<IBusinessLogicResult<UserSignInViewModel>> FindUserAsync(int userId);
        Task<IBusinessLogicResult> UpdateUserLastActivityDateAsync(int userId);
        Task<IBusinessLogicResult> SendVerificationEmailAsync(EmailViewModel emailViewModel, int activationCode);
        Task<IBusinessLogicResult> VerifyActivationCodeAysnc(ActivationCodeViewModel activationCodeViewModel);
        Task<IBusinessLogicResult<UserSignInViewModel>> UpdateUserRegisterInfoAsync(UserRegisterViewModel userRegisterViewModel);
        Task<IBusinessLogicResult<AddMerchantViewModel>> AddMerchantAsync(int userId, AddMerchantViewModel addMerchantViewModel);
        Task<IBusinessLogicResult<AddTransporterViewModel>> AddTransporterAsync(int userId, AddTransporterViewModel addTransporterViewModel);
        Task<IBusinessLogicResult> DeactivateUserAsync(int userId, int deactivatorUserId);
        Task<IBusinessLogicResult> ActivateUserAsync(int userId, int activatorUserId);
        Task<IBusinessLogicResult> ForgetPasswordAsync(UserForgetPasswordViewModel userForgetPasswordViewModel);
        Task<IBusinessLogicResult<int?>> MerchantAuthenticator(int userId);
        Task<IBusinessLogicResult<int?>> TransporterAuthenticator(int userId);
        Task<IBusinessLogicResult<UploadedPhotoViewModel>> UploadPhotoAsync(IFormFile formFile);
        Task<IBusinessLogicResult> ResendSendVerificationEmailAsync(EmailViewModel emailViewModel);
    }
}
