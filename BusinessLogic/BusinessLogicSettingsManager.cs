using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Cross.Abstractions.EntityEnums;
using Data.Abstractions;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic
{
    public class BusinessLogicSettingsManager: IBusinessLogicSettingsManager
    {
        private readonly IRepository<Settings> _settingsRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<HowItWorks> _howItWorksRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicSettingsManager(IRepository<Settings> settingsRepository, BusinessLogicUtility utility,
            IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository, IRepository<HowItWorks> howItWorksRepository)
        {
            _settingsRepository = settingsRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _howItWorksRepository = howItWorksRepository;
            _utility = utility;
        }
        public async Task<IBusinessLogicResult<SettingsViewModels>> AdminGetSettingsForEdit(int getterUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<SettingsViewModels>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModels>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Settings settings;
                try
                {
                    settings = _settingsRepository.DeferredSelectAll().FirstOrDefault();
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModels>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                SettingsViewModels settingsViewModel = await _utility.MapAsync<Settings, SettingsViewModels>(settings);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.Successed));
                return new BusinessLogicResult<SettingsViewModels>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<SettingsViewModels>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<IndexSettingsViewModel>> GetIndexSettings()
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                Settings settings;
                try
                {
                    settings = _settingsRepository.DeferredSelectAll().FirstOrDefault();
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<IndexSettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                IndexSettingsViewModel settingsViewModel = await _utility.MapAsync<Settings, IndexSettingsViewModel>(settings);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.Successed));
                return new BusinessLogicResult<IndexSettingsViewModel>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<IndexSettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<EditHowItWorksViewModel>> AdminGetHowItWorksForEditAsync(int getterUserId, int howItWorksId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                HowItWorks howItWorks;
                try
                {
                    howItWorks = await _howItWorksRepository.FindAsync(howItWorksId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                EditHowItWorksViewModel editHowItWorksViewModel = await _utility.MapAsync<HowItWorks, EditHowItWorksViewModel>(howItWorks);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.Successed));
                return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: true, messages: messages, result: editHowItWorksViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<TermsAndConditionsViewModel>> GetTermsAndConditions()
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                Settings settings;
                try
                {
                    settings = _settingsRepository.DeferredSelectAll().FirstOrDefault();
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<TermsAndConditionsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                TermsAndConditionsViewModel settingsViewModel = await _utility.MapAsync<Settings, TermsAndConditionsViewModel>(settings);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.Successed));
                return new BusinessLogicResult<TermsAndConditionsViewModel>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<TermsAndConditionsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<SettingsViewModels>> AdminEditSettings(int editorUserId, SettingsViewModels settingsViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == editorUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<SettingsViewModels>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModels>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Settings settings;
                try
                {
                    settings = _settingsRepository.DeferredSelectAll().FirstOrDefault();
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModels>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                await _utility.MapAsync(settingsViewModel, settings);
                try
                {
                    await _settingsRepository.UpdateAsync(settings, true);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModels>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.SettingSuccessfullySaved));
                return new BusinessLogicResult<SettingsViewModels>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<SettingsViewModels>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<HowItWorksViewModel>> AdminAddHowItWorksAsync(int adderUserId, HowItWorksViewModel howItWorksViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == adderUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Settings settings;
                try
                {
                    settings = _settingsRepository.DeferredSelectAll().FirstOrDefault();
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                HowItWorks howItWorks;
                howItWorks = await _utility.MapAsync<HowItWorksViewModel, HowItWorks>(howItWorksViewModel);
                howItWorks.SettingsId = settings.Id;

                try
                {
                    await _howItWorksRepository.AddAsync(howItWorks);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.EntitySuccessfullyAdded));
                return new BusinessLogicResult<HowItWorksViewModel>(succeeded: true, messages: messages, result: howItWorksViewModel);

            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<EditHowItWorksViewModel>> AdminEditHowItWorksAsync(int editorUserId, EditHowItWorksViewModel editHowItWorksViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == editorUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                HowItWorks howItWorks;
                try
                {
                    howItWorks = await _howItWorksRepository.FindAsync(editHowItWorksViewModel.Id);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                if (howItWorks == null)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, messages: messages, result: null);
                }

                howItWorks = await _utility.MapAsync<EditHowItWorksViewModel, HowItWorks>(editHowItWorksViewModel);

                try
                {
                    await _howItWorksRepository.UpdateAsync(howItWorks, true);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.EntitySuccessfullyUpdated));
                return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: true, messages: messages, result: editHowItWorksViewModel);

            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<EditHowItWorksViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<HowItWorksViewModel>> AdminDeleteHowItWorksAsync(int deleterUserId, int howItWorksId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                HowItWorks howItWorks;
                try
                {
                    howItWorks = await _howItWorksRepository.FindAsync(howItWorksId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                try
                {
                    await _howItWorksRepository.DeleteAsync(howItWorks, true);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }

                HowItWorksViewModel howItWorksViewModel = await _utility.MapAsync<HowItWorks, HowItWorksViewModel>(howItWorks);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.EntitySuccessfullyDeleted));
                return new BusinessLogicResult<HowItWorksViewModel>(succeeded: true, messages: messages, result: howItWorksViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
            }
        }

        public async Task<IBusinessLogicResult<ListResultViewModel<ListHowItWorksViewModel>>> ListHowItWorksAsync()
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                var listHowItWorksViewModel = await _howItWorksRepository.DeferredSelectAll()
                    .ProjectTo<ListHowItWorksViewModel>(new MapperConfiguration(config =>
                        config.CreateMap<HowItWorks, ListHowItWorksViewModel>())).ToListAsync();

                var result = new ListResultViewModel<ListHowItWorksViewModel>
                {
                    Results = listHowItWorksViewModel
                };

                return new BusinessLogicResult<ListResultViewModel<ListHowItWorksViewModel>>(succeeded: true,
                    result: result, messages: messages);

            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<ListHowItWorksViewModel>>(succeeded: false,
                    result: null, messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            _settingsRepository.Dispose();
            _userRoleRepository.Dispose();
            _roleRepository.Dispose();
        }
    }
}
