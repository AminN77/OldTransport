using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Cross.Abstractions;
using Cross.Abstractions.EntityEnums;
using Data.Abstractions;
using Data.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ViewModels;

namespace BusinessLogic
{
    public class BusinessLogicUserManager : IBusinessLogicUserManager, IUserAuthenticator
    {
        // Variables
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IRepository<Accept> _acceptRepository;
        private readonly IRepository<Transporter> _transporterRepository;
        private readonly IRepository<Offer> _offerRepository;
        private readonly BusinessLogicUtility _utility;
        private readonly ILogger<BusinessLogicUserManager> _logger;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISecurityProvider _securityProvider;
        private readonly IEmailSender _emailSender;
        private readonly IFileService _fileService;

        // Constructor
        public BusinessLogicUserManager(IRepository<User> userRepository, ILogger<BusinessLogicUserManager> logger,
                BusinessLogicUtility utility, IRepository<UserRole> userRoleRepository, IRepository<Merchant> merchantRepository,
                IPasswordHasher passwordHasher, ISecurityProvider securityProvider, IEmailSender emailSender, IFileService fileService,
                IRepository<Transporter> transporterRepository, IRepository<Role> roleRepository, IRepository<Accept> acceptRepository,
                IRepository<Offer> offerRepository)
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
            _roleRepository = roleRepository;
            _acceptRepository = acceptRepository;
            _offerRepository = offerRepository;
            _fileService = fileService;
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
                var developerUser = await _userRepository
                    .DeferredWhere(user => user.Id == getterUserId && user.IsEnabled)
                    .Join(_userRoleRepository.DeferredSelectAll(), user => user.Id, userRole => userRole.UserId,
                        (user, userRole) => userRole)
                    .Join(_roleRepository.DeferredSelectAll(), userRole => userRole.RoleId, role => role.Id,
                        (userRole, role) => role).AnyAsync(role => role.Name == RoleTypes.DeveloperSupport.ToString());

                var usersQuery = _userRepository.DeferredWhere(u => (!u.IsDeleted && !developerUser) || developerUser)
                    .Join(_userRoleRepository.DeferredSelectAll(),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                        .Join(_roleRepository.DeferredSelectAll(),
                        c => c.userRole.RoleId,
                        role => role.Id,
                        (c, role) => new ListUserViewModel()
                        {
                            Name = c.user.Name,
                            IsEnabled = c.user.IsEnabled,
                            Picture = c.user.Picture,
                            LastLoggedIn = c.user.LastLoggedIn,
                            EmailAddress = c.user.EmailAddress,
                            Id = c.user.Id,
                            Role = role.Name,
                            PhoneNumber = c.user.PhoneNumber
                        });


                //_userRepository.DeferredWhere(u =>
                //    (!u.IsDeleted && !developerUser) || developerUser
                //)
                //.ProjectTo<ListUserViewModel>(new MapperConfiguration(config =>
                //    config.CreateMap<User, ListUserViewModel>().ForMember(u => u.IsAdmin, o => o.MapFrom(l => l.UserRoles))));

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
                var pageCount = (int)Math.Ceiling(recordsCount / (double)pageSize);
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

        public async Task<IBusinessLogicResult<ListResultViewModel<ListMerchantViewModel>>> GetMerchantsAsync(int getterUserId,
            int page = 1,
            int pageSize = BusinessLogicSetting.MediumDefaultPageSize, string search = null,
            string sort = nameof(ListMerchantViewModel.Name) + ":Asc", string filter = null)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var getterUser = await _userRepository.FindAsync(getterUserId);
                // Solution 1:
                var developerUser = await _userRepository
                    .DeferredWhere(user => user.Id == getterUserId && user.IsEnabled)
                    .Join(_userRoleRepository.DeferredSelectAll(), user => user.Id, userRole => userRole.UserId,
                        (user, userRole) => userRole)
                    .Join(_roleRepository.DeferredSelectAll(), userRole => userRole.RoleId, role => role.Id,
                        (userRole, role) => role).AnyAsync(role => role.Name == RoleTypes.DeveloperSupport.ToString());

                var usersQuery = _userRepository.DeferredWhere(u => (!u.IsDeleted && !developerUser) || developerUser)
                    .Join(_userRoleRepository.DeferredSelectAll(),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                        .Join(_roleRepository.DeferredSelectAll(),
                        c => c.userRole.RoleId,
                        role => role.Id,
                        (c, role) => new { c, role })
                            .Join(_merchantRepository.DeferredSelectAll(),
                            d => d.c.user.Id,
                            merchant => merchant.UserId,
                            (d, merchant) => new ListMerchantViewModel()
                            {
                                Name = d.c.user.Name,
                                IsEnabled = d.c.user.IsEnabled,
                                Picture = d.c.user.Picture,
                                LastLoggedIn = d.c.user.LastLoggedIn,
                                EmailAddress = d.c.user.EmailAddress,
                                Id = d.c.user.Id,
                                Role = d.role.Name,
                                PhoneNumber = d.c.user.PhoneNumber
                            });


                //_userRepository.DeferredWhere(u =>
                //    (!u.IsDeleted && !developerUser) || developerUser
                //)
                //.ProjectTo<ListUserViewModel>(new MapperConfiguration(config =>
                //    config.CreateMap<User, ListUserViewModel>().ForMember(u => u.IsAdmin, o => o.MapFrom(l => l.UserRoles))));

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
                    sort = nameof(ListMerchantViewModel.Name) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListUserViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(ListMerchantViewModel.Name) + ":Asc";
                }

                usersQuery = usersQuery.ApplyOrderBy(sort);
                var userListViewModels = await usersQuery.PaginateAsync(page, pageSize);
                var recordsCount = await usersQuery.CountAsync();
                var pageCount = (int)Math.Ceiling(recordsCount / (double)pageSize);
                var result = new ListResultViewModel<ListMerchantViewModel>
                {
                    Results = userListViewModels,
                    Page = page,
                    PageSize = pageSize,
                    TotalEntitiesCount = recordsCount,
                    TotalPagesCount = pageCount
                };
                return new BusinessLogicResult<ListResultViewModel<ListMerchantViewModel>>(succeeded: true,
                    result: result, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<ListMerchantViewModel>>(succeeded: false,
                    result: null, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<ListResultViewModel<ListTransporterViewModel>>> GetTransportersAsync(int getterUserId,
            int page = 1,
            int pageSize = BusinessLogicSetting.MediumDefaultPageSize, string search = null,
            string sort = nameof(ListTransporterViewModel.Name) + ":Asc", string filter = null)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var getterUser = await _userRepository.FindAsync(getterUserId);
                // Solution 1:
                var developerUser = await _userRepository
                    .DeferredWhere(user => user.Id == getterUserId && user.IsEnabled)
                    .Join(_userRoleRepository.DeferredSelectAll(), user => user.Id, userRole => userRole.UserId,
                        (user, userRole) => userRole)
                    .Join(_roleRepository.DeferredSelectAll(), userRole => userRole.RoleId, role => role.Id,
                        (userRole, role) => role).AnyAsync(role => role.Name == RoleTypes.DeveloperSupport.ToString());

                var usersQuery = _userRepository.DeferredWhere(u => (!u.IsDeleted && !developerUser) || developerUser)
                    .Join(_userRoleRepository.DeferredSelectAll(),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => new { user, userRole })
                        .Join(_roleRepository.DeferredSelectAll(),
                        c => c.userRole.RoleId,
                        role => role.Id,
                        (c, role) => new { c, role })
                            .Join(_transporterRepository.DeferredSelectAll(),
                            d => d.c.user.Id,
                            transporter => transporter.UserId,
                            (d, transporter) => new ListTransporterViewModel()
                            {
                                Name = d.c.user.Name,
                                IsEnabled = d.c.user.IsEnabled,
                                Picture = d.c.user.Picture,
                                LastLoggedIn = d.c.user.LastLoggedIn,
                                EmailAddress = d.c.user.EmailAddress,
                                Id = d.c.user.Id,
                                Role = d.role.Name,
                                PhoneNumber = d.c.user.PhoneNumber
                            });


                //_userRepository.DeferredWhere(u =>
                //    (!u.IsDeleted && !developerUser) || developerUser
                //)
                //.ProjectTo<ListUserViewModel>(new MapperConfiguration(config =>
                //    config.CreateMap<User, ListUserViewModel>().ForMember(u => u.IsAdmin, o => o.MapFrom(l => l.UserRoles))));

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
                    sort = nameof(ListTransporterViewModel.Name) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListTransporterViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(ListUserViewModel.Name) + ":Asc";
                }

                usersQuery = usersQuery.ApplyOrderBy(sort);
                var userListViewModels = await usersQuery.PaginateAsync(page, pageSize);
                var recordsCount = await usersQuery.CountAsync();
                var pageCount = (int)Math.Ceiling(recordsCount / (double)pageSize);
                var result = new ListResultViewModel<ListTransporterViewModel>
                {
                    Results = userListViewModels,
                    Page = page,
                    PageSize = pageSize,
                    TotalEntitiesCount = recordsCount,
                    TotalPagesCount = pageCount
                };
                return new BusinessLogicResult<ListResultViewModel<ListTransporterViewModel>>(succeeded: true,
                    result: result, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<ListTransporterViewModel>>(succeeded: false,
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
                User user;
                // Verification
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
                    userChangePasswordViewModel.NewPassword, user.Salt, user.IterationCount,
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
        public async Task<IBusinessLogicResult> DeleteUserAsync(int? userId, int deleterUserId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    if (userId != null)
                    {
                        var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                        var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId != userRole.Id);
                        if (userId != deleterUserId && !isUserAuthorized)
                        {
                            messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                            return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                                messages: messages);
                        }
                    }
                    else
                    {
                        var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                        var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId == userRole.Id);
                        if (!isUserAuthorized)
                        {
                            messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                            return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                                messages: messages);
                        }
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
                    if (userId != null)
                    {
                        user = await _userRepository.FindAsync(userId);
                        if (user == null || user.IsDeleted)
                        {
                            messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                                BusinessLogicSetting.UserDisplayName));
                            return new BusinessLogicResult(succeeded: false, messages: messages);
                        }
                    }
                    else
                    {
                        user = await _userRepository.FindAsync(deleterUserId);
                        if (user == null || user.IsDeleted)
                        {
                            messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                                BusinessLogicSetting.UserDisplayName));
                            return new BusinessLogicResult(succeeded: false, messages: messages);
                        }
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                // Check developer & admin role
                var developerRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.DeveloperSupport.ToString());
                var adminRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.Admin.ToString());
                bool isUserDeveloper;
                bool isUserAdmin;
                if (userId != null)
                {
                    isUserDeveloper = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == developerRole.Id);
                    isUserAdmin = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == adminRole.Id);
                }
                else
                {
                    isUserDeveloper = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId == developerRole.Id);
                    isUserAdmin = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId == adminRole.Id);
                }
                if (isUserAdmin || isUserDeveloper)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                    return new BusinessLogicResult(succeeded: false, messages: messages);
                }

                try
                {
                    var merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == user.Id);
                    if (merchant != null)
                    {
                        bool hasActiveProject = _acceptRepository.DeferredSelectAll(a => a.MerchantId == merchant.Id).Where(a => a.Status == AcceptStatus.Accepted ||
                            a.Status == AcceptStatus.Delivered || a.Status == AcceptStatus.Loading || a.Status == AcceptStatus.Shipping).Any();

                        if (hasActiveProject)
                        {
                            messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.CannotDeleteAccountDueToActiveProject));
                            return new BusinessLogicResult(succeeded: false, messages: messages);
                        }
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                try
                {
                    var transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.UserId == user.Id);
                    if (transporter != null)
                    {
                        bool hasActiveProject = _acceptRepository.DeferredSelectAll()
                            .Join(_offerRepository.DeferredWhere(o => o.TransporterId == transporter.Id),
                            a => a.OfferId,
                            o => o.Id,
                            (a, o) => a).Where(a => a.Status == AcceptStatus.Accepted || a.Status == AcceptStatus.Delivered ||
                            a.Status == AcceptStatus.Loading || a.Status == AcceptStatus.Shipping).Any();

                        if (hasActiveProject)
                        {
                            messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.CannotDeleteAccountDueToActiveProject));
                            return new BusinessLogicResult(succeeded: false, messages: messages);
                        }
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
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
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == editorUserId && u.RoleId == userRole.Id);
                    if (editUserViewModel.Id != editorUserId && !isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // Check developer & admin role
                var developerRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.DeveloperSupport.ToString());
                var isUserDeveloper = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == editUserViewModel.Id && u.RoleId == developerRole.Id);
                var adminRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.Admin.ToString());
                var isUserAdmin = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == editUserViewModel.Id && u.RoleId == adminRole.Id);
                if (isUserAdmin || isUserDeveloper)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                User user;

                try
                {
                    user = await _userRepository.FindAsync(editUserViewModel.Id);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (user == null || user.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.EntityDoesNotExist,
                        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                // Safe
                user = await _utility.MapAsync(editUserViewModel, user);


                // Set new serial number
                user.SerialNumber = Guid.NewGuid().ToString();

                try
                {
                    await _userRepository.UpdateAsync(user, true); //, propertiesToBeUpdate.ToArray()
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                messages.Add(
                    new BusinessLogicMessage(type: MessageType.Info, message: MessageId.UserSuccessfullyEdited));
                return new BusinessLogicResult<EditUserViewModel>(succeeded: true, result: editUserViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<DetailUserViewModel>> GetUserDetailsAsync(int? userId, int getterUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    if (userId != null)
                    {
                        var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                        var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId != userRole.Id);

                        if (userId != getterUserId && !isUserAuthorized)
                        {
                            messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                            return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                                messages: messages);
                        }
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
                    if (userId != null)
                    {
                        user = await _userRepository.FindAsync(userId);
                    }
                    else
                    {
                        user = await _userRepository.FindAsync(getterUserId);
                    }
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
                var userDetailsViewModel = await _utility.MapAsync<User, DetailUserViewModel>(user);

                try
                {
                    if (userId != null)
                    {
                        var merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == userId);
                        if (merchant == null)
                        {
                            var transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.UserId == userId);
                            userDetailsViewModel.Bio = transporter.Bio;
                        }
                        else
                        {
                            userDetailsViewModel.Bio = merchant.Bio;
                        }
                    }
                    else
                    {
                        var merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == getterUserId);
                        if (merchant == null)
                        {
                            var transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.UserId == getterUserId);
                            userDetailsViewModel.Bio = transporter.Bio;
                        }
                        else
                        {
                            userDetailsViewModel.Bio = merchant.Bio;
                        }
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                return new BusinessLogicResult<DetailUserViewModel>(succeeded: true, result: userDetailsViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<EditUserViewModel>> GetUserForEditAsync(int getterUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId == userRole.Id);

                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                User user;
                // Critical Database
                try
                {
                    user = await _userRepository.FindAsync(getterUserId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // user Verification
                if (user == null || user.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.EntityDoesNotExist,
                        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                //safe Map
                var editUserViewModel = await _utility.MapAsync<User, EditUserViewModel>(user);

                try
                {
                    var merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == getterUserId);
                    if (merchant == null)
                    {
                        var transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.UserId == getterUserId);
                        editUserViewModel.Bio = transporter.Bio;
                    }
                    else
                    {
                        editUserViewModel.Bio = merchant.Bio;
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                return new BusinessLogicResult<EditUserViewModel>(succeeded: true, result: editUserViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
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
                    if (user == null || user.IsDeleted)
                    {
                        Exception exception = new Exception("");
                        messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EmailDoesNotExist));
                        return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                    }
                    if (!user.IsEnabled)
                    {
                        Exception exception = new Exception("DeactivatedUser");
                        messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.DeactivatedUser));
                        return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
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
                var res = await _emailSender.Send(emailViewModel.EmailAddress, "Dear User", activationCode.ToString());
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

        public async Task<IBusinessLogicResult> ResendSendVerificationEmailAsync(EmailViewModel emailViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                User user;
                try
                {
                    user = await _userRepository.DeferredSelectAll().SingleOrDefaultAsync(u => u.EmailAddress == emailViewModel.EmailAddress);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                if (user != null && !user.IsEnabled)
                {
                    await SendVerificationEmailAsync(emailViewModel, user.ActivationCode);

                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.VerificationEmailSuccessfullySent));
                    return new BusinessLogicResult(succeeded: true, messages: messages);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                return new BusinessLogicResult(succeeded: false, messages: messages);

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
                if (user == null || user.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult(succeeded: false, messages: messages);
                }

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
                    user.ActivationCode = new Random().Next(10001, 99999);
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
                if (user == null || user.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, messages: messages, result: null);
                }
                if (!user.IsEnabled)
                {
                    Exception exception = new Exception("DeactivatedUser");
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.DeactivatedUser));
                    return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

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

                var addMerchantViewModel = new AddMerchantViewModel();

                var addTransporterViewModel = new AddTransporterViewModel();

                if (userRegisterViewModel.Role)
                {
                    var res = await AddMerchantAsync(user.Id, addMerchantViewModel);
                    if (!res.Succeeded)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null, messages: messages);
                    }
                }
                else
                {
                    var res = await AddTransporterAsync(user.Id, addTransporterViewModel);
                    if (!res.Succeeded)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                        return new BusinessLogicResult<UserSignInViewModel>(succeeded: false, result: null, messages: messages);
                    }
                }

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

        public async Task<IBusinessLogicResult<AddMerchantViewModel>> AddMerchantAsync(int userId, AddMerchantViewModel addMerchantViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                bool doesExist;
                try
                {
                    doesExist = _merchantRepository.DeferredSelectAll().Any(t => t.UserId == userId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddMerchantViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                if (doesExist)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EntitiesFieldsValueAlreadyExisted));
                    return new BusinessLogicResult<AddMerchantViewModel>(succeeded: false, messages: messages, result: null);
                }
                //var merchant = await _utility.MapAsync<AddMerchantViewModel, Merchant>(addMerchantViewModel);
                Merchant merchant = new Merchant()
                {
                    UserId = userId
                };
                try
                {
                    await _merchantRepository.AddAsync(merchant);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddMerchantViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == userId);
                    addMerchantViewModel.MerchantId = merchant.Id;
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EntitySuccessfullyAdded));
                    return new BusinessLogicResult<AddMerchantViewModel>(succeeded: true, messages: messages, result: addMerchantViewModel);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddMerchantViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<AddMerchantViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<AddTransporterViewModel>> AddTransporterAsync(int userId, AddTransporterViewModel addTransporterViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                bool doesExist;
                try
                {
                    doesExist = _transporterRepository.DeferredSelectAll().Any(t => t.UserId == userId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddTransporterViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                if (doesExist)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EntitiesFieldsValueAlreadyExisted));
                    return new BusinessLogicResult<AddTransporterViewModel>(succeeded: false, messages: messages, result: null);
                }
                //var transporter = await _utility.MapAsync<AddTransporterViewModel, Transporter>(addTransporterViewModel);
                Transporter transporter = new Transporter()
                {
                    UserId = userId
                };
                try
                {
                    await _transporterRepository.AddAsync(transporter);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddTransporterViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                try
                {
                    transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.UserId == userId);
                    addTransporterViewModel.TransporterId = transporter.Id;
                    messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.EntitySuccessfullyAdded));
                    return new BusinessLogicResult<AddTransporterViewModel>(succeeded: true, messages: messages, result: addTransporterViewModel);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddTransporterViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<AddTransporterViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult> DeactivateUserAsync(int userId, int deactivatorUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deactivatorUserId && u.RoleId != userRole.Id);
                    if (userId != deactivatorUserId && !isUserAuthorized)
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

                // Check developer & admin role
                var developerRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.DeveloperSupport.ToString());
                var isUserDeveloper = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == developerRole.Id);
                var adminRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.Admin.ToString());
                var isUserAdmin = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == adminRole.Id);
                if (isUserAdmin || isUserDeveloper)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                user.IsEnabled = false;
                try
                {
                    await _userRepository.UpdateAsync(user);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.UserSuccessfullyDeactivated));
                return new BusinessLogicResult(succeeded: true, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> ActivateUserAsync(int userId, int activatorUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == activatorUserId && u.RoleId != userRole.Id);
                    if (userId != activatorUserId && !isUserAuthorized)
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

                user.IsEnabled = true;
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
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> ForgetPasswordAsync(UserForgetPasswordViewModel userForgetPasswordViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                User user;

                try
                {
                    user = await _userRepository.DeferredSelectAll().SingleOrDefaultAsync
                   (u => u.EmailAddress == userForgetPasswordViewModel.EmailAddress);
                }

                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);

                }

                if (user == null || user.IsEnabled == false || user.IsDeleted == true)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult(succeeded: false, messages: messages);
                }

                var Password = Guid.NewGuid().ToString("d").Substring(1, 8);

                try
                {
                    user.Password = await _securityProvider.PasswordHasher.HashPasswordAsync(
                    Password.ToString(), user.Salt, user.IterationCount,
                    BusinessLogicSetting.DefaultPassworHashCount);

                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);

                }

                try
                {
                    var EmailRes = await _emailSender.Send(userForgetPasswordViewModel.EmailAddress, "New Password", Password);

                    if (!EmailRes)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.CanNotSendEmail));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }

                }

                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                try
                {
                    await _userRepository.UpdateAsync(user, true); //, nameof(user.Password)
                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                        message: MessageId.PasswordSuccessfullyChanged));
                    return new BusinessLogicResult<UserForgetPasswordViewModel>(succeeded: true,
                        result: userForgetPasswordViewModel, messages: messages);

                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EmailDoesNotExist));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

            }

            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<int?>> MerchantAuthenticator(int userId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<int?>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<int?>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Merchant merchant;
                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(u => u.UserId == userId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<int?>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                if (merchant != null)
                {
                    int merchantId = merchant.Id;
                    messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.MerchantExists));
                    return new BusinessLogicResult<int?>(succeeded: true, messages: messages, result: merchantId);
                }
                else
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult<int?>(succeeded: false, messages: messages, result: null);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<int?>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<int?>> TransporterAuthenticator(int userId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == userId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<int?>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<int?>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Transporter transporter;
                try
                {
                    transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(u => u.UserId == userId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<int?>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                if (transporter != null)
                {
                    var transporterId = transporter.Id;
                    messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.TransporterExists));
                    return new BusinessLogicResult<int?>(succeeded: true, messages: messages, result: transporterId);
                }
                else
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult<int?>(succeeded: false, messages: messages, result: null);
                }
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<int?>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<UploadedPhotoViewModel>> UploadPhotoAsync(IFormFile formFile)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                bool checkFileType;
                try
                {
                    checkFileType = _fileService.FileTypeValidator(formFile, FileTypes.Photo);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<UploadedPhotoViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                if (checkFileType)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InvalidFileType));
                    return new BusinessLogicResult<UploadedPhotoViewModel>(succeeded: false, messages: messages, result: null);
                }

                UploadedPhotoViewModel uploadedPhotoViewModel = null;
                try
                {
                    uploadedPhotoViewModel.Picture = _fileService.SaveFile(formFile, FileTypes.Photo);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<UploadedPhotoViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                if (uploadedPhotoViewModel == null)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<UploadedPhotoViewModel>(succeeded: false, messages: messages, result: null);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.FileSuccessfullyUploaded));
                return new BusinessLogicResult<UploadedPhotoViewModel>(succeeded: true, messages: messages, result: uploadedPhotoViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<UploadedPhotoViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public void Dispose()
        {
            _merchantRepository.Dispose();
            _roleRepository.Dispose();
            _userRoleRepository.Dispose();
            _userRepository.Dispose();
            _transporterRepository.Dispose();
        }
    }
}