using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Cross.Abstractions;
using Data.Abstractions;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewModels;

namespace BusinessLogic
{
    public class BusinessLogicUserManager : IBusinessLogicUserManager, IUserAuthenticator
    {
        // Variables
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IRepository<Transporter> _transporterRepository;
        private readonly BusinessLogicUtility _utility;
        private readonly ILogger<BusinessLogicUserManager> _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISecurityProvider _securityProvider;
        private readonly IEmailSender _emailSender;

        // Constructor
        public BusinessLogicUserManager(IRepository<User> userRepository, ILogger<BusinessLogicUserManager> logger,
                BusinessLogicUtility utility, IRepository<UserRole> userRoleRepository, IRepository<Merchant> merchantRepository,
                IPasswordHasher passwordHasher, ISecurityProvider securityProvider, IEmailSender emailSender, IRepository<Transporter> transporterRepository)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _logger = logger;
            _utility = utility;
            _passwordHasher = passwordHasher;
            _securityProvider = securityProvider;
            _emailSender = emailSender;
            _merchantRepository = merchantRepository;
            _transporterRepository = transporterRepository;
        }

        #region User

        /// <summary>
        /// Add User Async
        /// </summary>
        /// <param name="addUserViewModel"></param>
        /// <param name="adderUserId"></param>
        /// <returns></returns>
        public async Task<IBusinessLogicResult<AddUserViewModel>> AddUserAsync(EmailViewModel emailViewModel)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                //// Validation Password
                //if (addUserViewModel.Password != addUserViewModel.PasswordConfirm)
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                //        message: MessageId.PasswordAndPasswordConfirmAreNotMached));
                //    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                //        messages: messages);
                //}

                //// Critical Validation Username
                //var isUserNameExisted = await _userRepository.DeferredSelectAll().AnyAsync(usr => usr.EmailAddress == addUserViewModel.EmailAddress);
                //if (isUserNameExisted)
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                //        message: MessageId.UsernameAlreadyExisted, viewMessagePlaceHolders: addUserViewModel.EmailAddress));
                //    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                //        messages: messages);
                //}

                //// Safe Map
                //var user = await _utility.MapAsync<AddUserViewModel, User>(addUserViewModel);

                // Safe initialization
                //user.AdderUserId = adderUserId;
                //user.LastEditorUserId = adderUserId;
                //user.AddedDateTime = DateTime.Now;
                //user.LastEditedDateTime = user.AddedDateTime;

                // Create password hash
                var user = await _utility.MapAsync<EmailViewModel, User>(emailViewModel);

                user.Name = "New User";
                user.Salt = await _securityProvider.GenerateRandomSaltAsync(BusinessLogicSetting.DefaultSaltCount);
                user.IterationCount = new Random().Next(BusinessLogicSetting.DefaultMinIterations,
                    BusinessLogicSetting.DefaultMaxIterations);
                user.ActivationCode = new Random().Next(10001, 99999);
                user.Password = await _securityProvider.PasswordHasher.HashPasswordAsync(user.ActivationCode.ToString(),
                    user.Salt, user.IterationCount, BusinessLogicSetting.DefaultPassworHashCount);

                user.CreateDateTime = DateTime.Now;

                var res = await SendVerificationEmailAsync(emailViewModel, user.ActivationCode);
                if (!res.Succeeded)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null, messages: messages, exception: res.Exception);
                }

                // Critical Database operation
                try
                {
                    await _userRepository.AddAsync(user);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                //AddUserViewModel addUserViewModel = new AddUserViewModel()
                //{
                //    EmailAddress = emailViewModel.EmailAddress,
                //    Password = user.ActivationCode.ToString(),
                //    ActivationCode = user.ActivationCode
                //};

                var addUserViewModel = await _utility.MapAsync<User, AddUserViewModel>(user);

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.EntitySuccessfullyAdded));
                return new BusinessLogicResult<AddUserViewModel>(succeeded: true, result: addUserViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        /// <summary>
        /// Get All Users Async
        /// </summary>
        /// <param name="getterUserId"></param>
        /// <returns></returns>
        public async Task<IBusinessLogicResult<ListResultViewModel<ListUserViewModel>>> GetUsersAsync(int getterUserId,
            int page = 1,
            int pageSize = BusinessLogicSetting.MediumDefaultPageSize, string search = null,
            string sort = nameof(ListUserViewModel.Name) + ":Asc", string filter = null)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {

                var getterUser = await _userRepository.FindAsync(getterUserId);
                // Solution 1:
                // Todo: Abolfazl -> set developer role type. (Abolfazl)
                const bool developerUser = true; // RoleType.DeveloperSupport;
                var usersQuery = _userRepository.DeferredWhere(u =>
                        (!u.IsDeleted && !developerUser) || developerUser
                    )
                    .ProjectTo<ListUserViewModel>(new MapperConfiguration(config =>
                        config.CreateMap<User, ListUserViewModel>()));
                if (!string.IsNullOrEmpty(search))
                {
                    usersQuery = usersQuery.Where(user =>
                        user.Name.Contains(search) || user.EmailAddress.Contains(search));
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    usersQuery = usersQuery.ApplyFilter(filter);
                }

                if (string.IsNullOrWhiteSpace(sort))
                {
                    sort = nameof(ListUserViewModel.Name) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListUserViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(ListUserViewModel.Name) + ":Asc";
                }

                usersQuery = usersQuery.ApplyOrderBy(sort);
                var userListViewModels = await usersQuery.PaginateAsync(page, pageSize);
                var recordsCount = await usersQuery.CountAsync();
                var pageCount = (int) Math.Ceiling(recordsCount / (double) pageSize);
                var result = new ListResultViewModel<ListUserViewModel>
                {
                    Results = userListViewModels,
                    Page = page,
                    PageSize = pageSize,
                    TotalEntitiesCount = recordsCount,
                    TotalPagesCount = pageCount
                };
                return new BusinessLogicResult<ListResultViewModel<ListUserViewModel>>(succeeded: true,
                    result: result, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<ListUserViewModel>>(succeeded: false,
                    result: null, messages: messages, exception: exception);
            }
        }

        /// <summary>
        /// Change Password Async
        /// </summary>
        /// <param name="userChangePasswordViewModel"></param>
        /// <param name="changerUserId"></param>
        /// <returns></returns>
        public async Task<IBusinessLogicResult> ChangePasswordAsync(
            UserChangePasswordViewModel userChangePasswordViewModel,
            int changerUserId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                if (userChangePasswordViewModel.Id != changerUserId)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                    return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                // Critical Authentication and Authorization
                User user;
                // user verification
                try
                {
                    user = await _userRepository.FindAsync(changerUserId);
                    if (user == null || !user.IsEnabled)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied));
                        return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // Validation Password
                if (userChangePasswordViewModel.NewPassword != userChangePasswordViewModel.PasswordConfirm)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.PasswordAndPasswordConfirmAreNotMached));
                    return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                if (user.Password != await _securityProvider.PasswordHasher.HashPasswordAsync(
                        userChangePasswordViewModel.OldPassword, user.Salt, user.IterationCount,
                        BusinessLogicSetting.DefaultPassworHashCount))
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InvalidPassword));
                    return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                // Create password hash
                //user.Salt = await _securityProvider.GenerateRandomSaltAsync(BusinessLogicSetting.DefaultSaltCount);
                //user.IterationCount = new Random().Next(BusinessLogicSetting.DefaultMinIterations,
                //BusinessLogicSetting.DefaultMaxIterations);
                user.Password = await _securityProvider.PasswordHasher.HashPasswordAsync(
                    userChangePasswordViewModel.OldPassword, user.Salt, user.IterationCount,
                    BusinessLogicSetting.DefaultPassworHashCount);
                // Critical Database operation
                try
                {
                    await _userRepository.UpdateAsync(user, true); //, nameof(user.Password)
                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                        message: MessageId.PasswordSuccessfullyChanged));
                    return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: true,
                        result: userChangePasswordViewModel, messages: messages);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<UserChangePasswordViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        /// <summary>
        /// Delete User Async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="deleterUserId"></param>
        /// <returns></returns>
        public async Task<IBusinessLogicResult> DeleteUserAsync(int userId, int deleterUserId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var isUserAdmin = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId != 2);
                    if (userId != deleterUserId || !isUserAdmin)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User verification
                User user;
                try
                {
                    user = await _userRepository.FindAsync(userId);
                    if (user == null || user.IsDeleted)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                // Check developer role
                var isUserDeveloper = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == 3);
                if (isUserDeveloper)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                // Critical Database operation
                try
                {
                    //-------------------------------------------------------------------
                    user.IsDeleted = true;
                    user.IsEnabled = false;
                    user.EmailAddress += "-deleted" +
                                     (await _userRepository
                                         .DeferredWhere(us => us.EmailAddress.Contains(user.EmailAddress + "-deleted"))
                                         .CountAsync()).ToString("D4");
                    user.Picture += "-deleted" +
                                        (await _userRepository.DeferredWhere(us =>
                                            us.Picture.Contains(user.Picture + "-deleted")).CountAsync())
                                        .ToString("D4");
                    user.SerialNumber = Guid.NewGuid().ToString();
                    //-------------------------------------------------------------------
                    await _userRepository.UpdateAsync(user, true);
                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                        message: MessageId.UserSuccessfullyDeleted));
                    return new BusinessLogicResult(succeeded: true, messages: messages);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        /// <summary>
        /// Edit User Async
        /// </summary>
        /// <param name="editUserViewModel"></param>
        /// <param name="editorUserId"></param>
        /// <returns></returns>
        public async Task<IBusinessLogicResult<EditUserViewModel>> EditUserAsync(EditUserViewModel editUserViewModel,
            int editorUserId)
        {
            return null;
//            var messages = new List<BusinessLogicMessage>();
//            try
//            {
//                // Critical Authentication and Authorization
////                // Critical Database
////
////                var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(editorUserId,
////                    UserManagerPermissions.EditUser.ToString());
////                if (!isUserInPermission.Succeeded) return isUserInPermission;
//
//                // Check organization level
////                var getterUser = await _userRepository.FindAsync(editorUserId);
////                var subLevelsId = await GetSubLevels(getterUser.OrganizationLevelId);
////                if (!(subLevelsId.Result.Contains(editUserViewModel.OrganizationLevelId) ||
////                      editUserViewModel.Id == editorUserId))
////                {
////                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
////                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
////                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
////                        messages: messages);
////                }
//
//                // User verification
//                var isUserNameExisted = await _userRepository.DeferredSelectAll(u => u.Id != editUserViewModel.Id)
//                    .AnyAsync(usr => usr.Username == editUserViewModel.Username);
//
//                // Check developer role
////                var developerRole = await _roleRepository.DeferredSelectAll()
////                    .SingleOrDefaultAsync(role => role.Name == RoleType.DeveloperSupport.ToString().ToLower());
////                if (editUserViewModel.RoleIds.Contains(developerRole.Id))
////                {
////                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
////                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
////                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
////                        messages: messages);
////                }
//
//                // Check Username Existed
//                if (isUserNameExisted)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
//                        message: MessageId.UsernameAlreadyExisted,
//                        viewMessagePlaceHolders: editUserViewModel.Username));
//                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
//                        messages: messages);
//                }
//
//                User user;
//
//                try
//                {
//                    user = await _userRepository.FindAsync(editUserViewModel.Id);
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages, exception: exception);
//                }
//
//                if (user == null || user.IsDeleted)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
//                        message: MessageId.EntityDoesNotExist,
//                        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages);
//                }
//
//                // Safe
//                user = await _utility.MapAsync(editUserViewModel, user);
//                var userId = user.Id;
//                var oldUserRoles = await _userRoleRepository.DeferredWhere(userRole => userRole.UserId == userId)
//                    .ToListAsync();
//                var userOldRoleIds = oldUserRoles.Select(userRole => userRole.RoleId).ToList();
//                if (editUserViewModel.RoleIds != null)
//                {
//                    var toBeDeletedUserRoleIds = userOldRoleIds.Except(editUserViewModel.RoleIds);
//                    var toBeDeletedUserRoles = oldUserRoles
//                        .Where(userRole => toBeDeletedUserRoleIds.Contains(userRole.RoleId)).ToList();
//                    await _userRoleRepository.DeleteAllAsync(toBeDeletedUserRoles, false);
//                    var toBeAddedUserRoleIds = editUserViewModel.RoleIds.Except(userOldRoleIds);
//                    foreach (var roleId in toBeAddedUserRoleIds)
//                    {
//                        var userRole = new UserRole
//                        {
//                            RoleId = roleId,
//                            UserId = user.Id
//                        };
//                        await _userRoleRepository.AddOrUpdateAsync(userRole, false);
//                    }
//                }
//
//                // Set new serial number
//                user.SerialNumber = Guid.NewGuid();
//
//                try
//                {
//                    await _userRepository.UpdateAsync(user, true); //, propertiesToBeUpdate.ToArray()
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages, exception: exception);
//                }
//
//                messages.Add(
//                    new BusinessLogicMessage(type: MessageType.Info, message: MessageId.UserSuccessfullyEdited));
//                return new BusinessLogicResult<EditUserViewModel>(succeeded: true, result: editUserViewModel,
//                    messages: messages);
//            }
//            catch (Exception exception)
//            {
//                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
//                return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                    messages: messages, exception: exception);
//            }
        }

        /// <summary>
        /// Edit User Is Enable Field Async
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="isEnabled"></param>
        /// <param name="setterUserId"></param>
        /// <returns></returns>
        public async Task<IBusinessLogicResult> EditUserIsEnabledAsync(int userId, bool isEnabled, int setterUserId)
        {
            return null;
//            var messages = new List<BusinessLogicMessage>();
//            try
//            {
//                // Critical Authentication and Authorization
//                // TODO: view model
//                var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(setterUserId,
//                    UserManagerPermissions.EditUser.ToString());
//                if (!isUserInPermission.Succeeded) return isUserInPermission;
////                }
////                catch (Exception exception)
////                {
////                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
////                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
////                        messages: messages, exception: exception);
////                }
//
//                // User verification
//                User user;
//                try
//                {
//                    user = await _userRepository.FindAsync(userId);
//                    if (user == null)
//                    {
//                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
//                            BusinessLogicSetting.UserDisplayName));
//                        return new BusinessLogicResult(succeeded: false, messages: messages);
//                    }
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
//                }
//
//                // Check organization level
//                var getterUser = await _userRepository.FindAsync(setterUserId);
//                var subLevelsId = await GetSubLevels(getterUser.OrganizationLevelId);
//                if (!subLevelsId.Result.Contains(user.OrganizationLevelId) || user.Id != setterUserId)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
//                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages);
//                }
//
//                // Critical Database operation
//                user.IsEnabled = isEnabled;
//                try
//                {
//                    await _userRepository.UpdateAsync(user, true); //, nameof(user.IsEnabled)
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
//                        message: MessageId.UserIsEnabledSuccessfullySet,
//                        viewMessagePlaceHolders: isEnabled ? "فعال" : "غیر فعال"));
//                    return new BusinessLogicResult(succeeded: true, messages: messages);
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
//                }
//            }
//            catch (Exception exception)
//            {
//                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
//                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
//            }
        }


        public async Task<IBusinessLogicResult<DetailUserViewModel>> GetUserDetailsAsync(int userId, int getterUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var isUserAdmin = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId != 2);
                    if (userId != getterUserId || !isUserAdmin)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                User user;
                // Critical Database
                try
                {
                    user = await _userRepository.FindAsync(userId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // user Verification
                if (user == null || user.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.EntityDoesNotExist,
                        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                //safe Map
                var userViewModel = await _utility.MapAsync<User, DetailUserViewModel>(user);
                return new BusinessLogicResult<DetailUserViewModel>(succeeded: true, result: userViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<EditUserViewModel>> GetUserForEditAsync(int userId, int getterUserId)
        {
            return null;
//            var messages = new List<IBusinessLogicMessage>();
//            try
//            {
//                // Critical Authentication and Authorization
//                var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(getterUserId,
//                    UserManagerPermissions.EditUser.ToString());
//                if (!isUserInPermission.Succeeded) return isUserInPermission;
//
//                User user;
//                // Critical Database
//                try
//                {
//                    user = await _userRepository.FindAsync(userId);
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages, exception: exception);
//                }
//
//                // User Verification
//                if (user == null || user.IsDeleted)
//                {
//                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
//                        BusinessLogicSetting.UserDisplayName));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages);
//                }
//
//                // Check organization level
//                var getterUser = await _userRepository.FindAsync(getterUserId);
//                var subLevelsId = await GetSubLevels(getterUser.OrganizationLevelId);
//                if (!(subLevelsId.Result.Contains(user.OrganizationLevelId) || user.Id == getterUserId))
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
//                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
//                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                        messages: messages);
//                }
//
//                // Safe Map
//                var userViewModel = await _utility.MapAsync<User, EditUserViewModel>(user);
//                if (userId != getterUserId)
//                    userViewModel.RoleIds = await _userRoleRepository
//                        .DeferredWhere(userRole => userRole.UserId == userId).Select(userRole => userRole.RoleId)
//                        .ToArrayAsync();
//                return new BusinessLogicResult<EditUserViewModel>(succeeded: true, result: userViewModel,
//                    messages: messages);
//            }
//            catch (Exception exception)
//            {
//                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
//                return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
//                    messages: messages, exception: exception);
//            }
        }


        public async Task<IBusinessLogicResult> IsUserNameAvailableAsync(string userName, int getterUserId)
        {
            return null;
//            var messages = new List<IBusinessLogicMessage>();
//            try
//            {
//                // Critical Authentication and Authorization
//                var isUserInPermission = await IsUserInOneOfPermissionsAsync(getterUserId,
//                    UserManagerPermissions.EditUser.ToString(), UserManagerPermissions.AddUser.ToString());
//                if (!isUserInPermission.Succeeded) return isUserInPermission;
//
//                bool isUserNameAvailable;
//                try
//                {
//                    isUserNameAvailable =
//                        !await _userRepository.DeferredSelectAll().AnyAsync(usr => usr.Username == userName);
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult(succeeded: false, messages: messages,
//                        exception: exception);
//                }
//
//                return new BusinessLogicResult(succeeded: isUserNameAvailable, messages: messages);
//            }
//            catch (Exception exception)
//            {
//                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
//                return new BusinessLogicResult(succeeded: false, messages: messages,
//                    exception: exception);
//            }
        }


        //public async Task<IBusinessLogicResult> ResetPasswordAsync(UserSetPasswordViewModel userSetPasswordViewModel,
        //    int reSetterUserId)
        //{
        //    return null;
//            var messages = new List<BusinessLogicMessage>();
//            try
//            {
//                // Critical Authentication and Authorization
//                var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(reseterUserId,
//                    UserManagerPermissions.EditUser.ToString());
//                if (!isUserInPermission.Succeeded) return isUserInPermission;
//
//                // user verification
//                User user;
//                try
//                {
//                    user = await _userRepository.FindAsync(userSetPasswordViewModel.Id);
//                    if (user == null)
//                    {
//                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
//                            BusinessLogicSetting.UserDisplayName));
//                        return new BusinessLogicResult<UserSetPasswordViewModel>(succeeded: false, result: null,
//                            messages: messages);
//                    }
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult<UserSetPasswordViewModel>(succeeded: false, result: null,
//                        messages: messages, exception: exception);
//                }
//
//                // Validation Password
//                if (userSetPasswordViewModel.NewPassword != userSetPasswordViewModel.PasswordConfirm)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
//                        message: MessageId.PasswordAndPasswordConfirmAreNotMached));
//                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
//                        messages: messages);
//                }
//
//                // Create password hash
//                //user.Salt = await _securityProvider.GenerateRandomSaltAsync(BusinessLogicSetting.DefaultSaltCount);
//                //user.IterationCount = new Random().Next(BusinessLogicSetting.DefaultMinIterations,
//                //    BusinessLogicSetting.DefaultMaxIterations);
//                user.Password = await _securityProvider.PasswordHasher.HashPasswordAsync(
//                    userSetPasswordViewModel.NewPassword, user.Salt, user.IterationCount,
//                    BusinessLogicSetting.DefaultPassworHashCount);
//                // Critical Database operation
//                try
//                {
//                    //await _userRepository.UpdateAsync(user, true, nameof(user.PasswordHash), nameof(user.Salt),
//                    //  nameof(user.IterationCount));
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
//                        message: MessageId.PasswordSuccessfullyReseted));
//                    return new BusinessLogicResult<UserSetPasswordViewModel>(succeeded: true,
//                        result: userSetPasswordViewModel, messages: messages);
//                }
//                catch (Exception exception)
//                {
//                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
//                    return new BusinessLogicResult<UserSetPasswordViewModel>(succeeded: false, result: null,
//                        messages: messages, exception: exception);
//                }
//            }
//            catch (Exception exception)
//            {
//                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
//                return new BusinessLogicResult<UserSetPasswordViewModel>(succeeded: false, result: null,
//                    messages: messages, exception: exception);
//            }
        //}


         public async Task<IBusinessLogicResult<UserSignInViewModel>> IsUserAuthenticateAsync(
            SignInInfoViewModel signInInfoViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Check username exist
                User user;
                try
                {
                    user = await _userRepository.DeferredSelectAll()
                        .SingleOrDefaultAsync(usr => usr.EmailAddress == signInInfoViewModel.EmailAddress && usr.IsEnabled);
                    if (user == null || user.IsDeleted || user.IsEnabled == false)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.UsernameOrPasswordInvalid,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false,
                            result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                        message: MessageId.InternalError));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // Check user password
                try
                {
                    var passwordHash = await _passwordHasher.HashPasswordAsync(signInInfoViewModel.Password, user.Salt,
                        user.IterationCount, 128);
                    if (user.Password != passwordHash)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                            message: MessageId.UsernameOrPasswordInvalid));
                        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                        message: MessageId.InternalError));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                // Mapping
                var userSignInViewModel = await _utility.MapAsync<User, UserSignInViewModel>(user);
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: userSignInViewModel != null,
                    result: userSignInViewModel, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                    message: MessageId.InternalError));
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }


         public async Task<IBusinessLogicResult<UserSignInViewModel>> FindUserAsync(int userId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Check username exist
                User user;
                try
                {
                    user = await _userRepository.FindAsync(userId);
                    if (user == null || user.IsDeleted || user.IsEnabled == false)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.UsernameOrPasswordInvalid,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false,
                            result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                        message: MessageId.InternalError));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // Mapping
                var userSignInViewModel = await _utility.MapAsync<User, UserSignInViewModel>(user);
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: userSignInViewModel != null,
                    result: userSignInViewModel, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                    message: MessageId.InternalError));
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }


        public async Task<IBusinessLogicResult> UpdateUserLastActivityDateAsync(int userId)
        {

            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Check username exist
                User user;
                try
                {
                    user = await _userRepository.FindAsync(userId);

                    if (user == null || user.IsDeleted || user.IsEnabled == false)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.UsernameOrPasswordInvalid,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false,
                            result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                        message: MessageId.InternalError));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (user.LastLoggedIn != null)
                {
                    var updateLastActivityDate = TimeSpan.FromMinutes(2);
                    var currentUtc = DateTimeOffset.UtcNow;
                    var timeElapsed = currentUtc.Subtract(user.LastLoggedIn.Value);
                    if (timeElapsed < updateLastActivityDate)
                    {
                        return new BusinessLogicResult(succeeded: false);
                    }
                }

                user.LastLoggedIn = DateTimeOffset.UtcNow;
                try
                {
                    await _userRepository.UpdateAsync(user);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                        message: MessageId.InternalError));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                return new BusinessLogicResult(succeeded: true);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical,
                    message: MessageId.InternalError));
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        #endregion

        public async Task<IBusinessLogicResult> DoesEmailExistAsync(EmailViewModel emailViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Check email existance
                try
                {
                    var user = await _userRepository.DeferredSelectAll().SingleOrDefaultAsync(usr => usr.EmailAddress == emailViewModel.EmailAddress);
                    if (user == null || user.IsDeleted || user.IsEnabled == false)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EmailDoesNotExist));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EmailSuccessfullyVerified));
                return new BusinessLogicResult(succeeded: true, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> SendVerificationEmailAsync(EmailViewModel emailViewModel, int activationCode)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var res = await _emailSender.Send(emailViewModel.EmailAddress, "Gikhar", activationCode.ToString());
                if (!res)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EmailSendingProcessFailed));
                    return new BusinessLogicResult(succeeded: false, messages: messages);
                }
                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.VerificationEmailSuccessfullySent));
                return new BusinessLogicResult(succeeded: true, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> VerifyActivationCodeAysnc(ActivationCodeViewModel activationCodeViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                User user = await _userRepository.DeferredSelectAll().SingleOrDefaultAsync(usr => usr.EmailAddress == activationCodeViewModel.EmailAddress);

                if (user.ActivationCode == activationCodeViewModel.ActivationCode)
                {
                    user.IsEnabled = true;
                    try
                    {
                        UserRole userRole = new UserRole()
                        {
                            UserId = user.Id,
                            RoleId = 2
                        };
                        await _userRoleRepository.AddAsync(userRole);
                    }
                    catch (Exception exception)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                        return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                    }
                    try
                    {
                        await _userRepository.UpdateAsync(user);
                    }
                    catch (Exception exception)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                        return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                    }
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.UserSuccessfullyActivated));
                    return new BusinessLogicResult(succeeded: true, messages: messages);
                }
                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.ActivationCodeVerficationFailed));
                return new BusinessLogicResult(succeeded: false, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<UserSignInViewModel>> UpdateUserRegisterInfoAsync(UserRegisterViewModel userRegisterViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var user = await _userRepository.DeferredSelectAll().SingleOrDefaultAsync(usr => usr.EmailAddress == userRegisterViewModel.EmailAddress);

                user.Name = userRegisterViewModel.Name;
                user.Password = await _securityProvider.PasswordHasher.HashPasswordAsync(userRegisterViewModel.Password,
                        user.Salt, user.IterationCount, BusinessLogicSetting.DefaultPassworHashCount);
                user.SerialNumber = Guid.NewGuid().ToString();
                try
                {
                    await _userRepository.UpdateAsync(user);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null, messages: messages, exception: exception);
                }

                var userIdViewModel = await _utility.MapAsync<User, UserIdViewModel>(user);

                //if (userRegisterViewModel.Role)
                //{
                //    var res = await AddMerchantAsync(userIdViewModel);
                //    if (!res.Succeeded)
                //    {
                //        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                //        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null, messages: messages);
                //    }
                //}
                //else
                //{
                //    var res = await AddTransporterAsync(userIdViewModel);
                //    if (!res.Succeeded)
                //    {
                //        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                //        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null, messages: messages);
                //    }
                //}

                // Safe Map
                var userSignInViewModel = await _utility.MapAsync<User, UserSignInViewModel>(user);

                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EntitySuccessfullyUpdated));
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: true, result: userSignInViewModel, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null, messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult> AddMerchantAsync(UserIdViewModel userIdViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var merchant = await _utility.MapAsync<UserIdViewModel, Merchant>(userIdViewModel);
                try
                {
                    await _merchantRepository.AddAsync(merchant);
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EntitySuccessfullyAdded));
                    return new BusinessLogicResult(succeeded: true, messages: messages);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> AddTransporterAsync(UserIdViewModel userIdViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var transporter = await _utility.MapAsync<UserIdViewModel, Transporter>(userIdViewModel);
                try
                {
                    await _transporterRepository.AddAsync(transporter);
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EntitySuccessfullyAdded));
                    return new BusinessLogicResult(succeeded: true, messages: messages);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            _userRepository.Dispose();
            _userRoleRepository.Dispose();
            _merchantRepository.Dispose();
            _transporterRepository.Dispose();
        }
    }
}