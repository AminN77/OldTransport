﻿using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicUserManager : IDisposable
    {
        Task<IBusinessLogicResult<AddUserViewModel>> AddUserAsync(AddUserViewModel addUserViewModel, int adderUserId);
        Task<IBusinessLogicResult<ListResultViewModel<ListUserViewModel>>> GetUsersAsync(int getterUserId, int page,
            int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<EditUserViewModel>> EditUserAsync(EditUserViewModel editUserViewModel, int editorUserId);
        Task<IBusinessLogicResult> DeleteUserAsync(int userId, int deleterUserId);
        Task<IBusinessLogicResult<EditUserViewModel>> GetUserForEditAsync(int userId, int getterUserId);
        Task<IBusinessLogicResult<DetailUserViewModel>> GetUserDetailsAsync(int userId, int getterUserId);
        Task<IBusinessLogicResult> EditUserIsEnabledAsync(int userId, bool isEnabled, int setterUserId);
        Task<IBusinessLogicResult> IsUserNameAvailableAsync(string userName, int getterUserId);
        Task<IBusinessLogicResult> ChangePasswordAsync(UserChangePasswordViewModel userChangePasswordViewModel, int changerUserId);
        Task<IBusinessLogicResult> ResetPasswordAsync(UserSetPasswordViewModel adminUserSetPasswordViewModel, int reSetterUserId);

    }
}