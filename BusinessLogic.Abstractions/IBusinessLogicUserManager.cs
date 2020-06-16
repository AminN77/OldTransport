using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicUserManager : IDisposable
    {
        Task<IBusinessLogicResult<AddUserViewModel>> AddUserAsync(EmailViewModel emailViewModel);
        Task<IBusinessLogicResult<ListResultViewModel<ListUserViewModel>>> GetUsersAsync(int getterUserId, int page,
            int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<EditUserViewModel>> EditUserAsync(EditUserViewModel editUserViewModel, int editorUserId, IFormFile file);
        Task<IBusinessLogicResult> DeleteUserAsync(int userId, int deleterUserId);
        Task<IBusinessLogicResult<EditUserViewModel>> GetUserForEditAsync(int userId, int getterUserId);
        Task<IBusinessLogicResult<DetailUserViewModel>> GetUserDetailsAsync(int userId, int getterUserId);
        Task<IBusinessLogicResult> IsUserNameAvailableAsync(string userName, int getterUserId);
        Task<IBusinessLogicResult> ChangePasswordAsync(UserChangePasswordViewModel userChangePasswordViewModel, int changerUserId);
        //Task<IBusinessLogicResult> ResetPasswordAsync(UserSetPasswordViewModel adminUserSetPasswordViewModel, int reSetterUserId);
        Task<IBusinessLogicResult<UserSignInViewModel>> FindUserAsync(int userId);
        Task<IBusinessLogicResult> UpdateUserLastActivityDateAsync(int userId);
        Task<IBusinessLogicResult> SendVerificationEmailAsync(EmailViewModel emailViewModel, int activationCode);
        Task<IBusinessLogicResult> VerifyActivationCodeAysnc(ActivationCodeViewModel activationCodeViewModel);
        Task<IBusinessLogicResult<UserSignInViewModel>> UpdateUserRegisterInfoAsync(UserRegisterViewModel userRegisterViewModel);
        Task<IBusinessLogicResult> AddMerchantAsync(AddMerchantViewModel addMerchantViewModel);
        Task<IBusinessLogicResult> AddTransporterAsync(AddTransporterViewModel addTransporterViewModel);
        Task<IBusinessLogicResult> DeactivateUserAsync(int userId, int deactivatorUserId);
        Task<IBusinessLogicResult> ActivateUserAsync(int userId, int activatorUserId);
        Task<IBusinessLogicResult> ForgetPasswordAsync(UserForgetPasswordViewModel userForgetPasswordViewModel);
        Task<IBusinessLogicResult<SettingsViewModel>> AdminGetSettings();
        Task<IBusinessLogicResult<SettingsViewModel>> AdminEditSettings(SettingsViewModel settingsViewModel);
    }
}
