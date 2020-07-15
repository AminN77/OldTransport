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
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicSettingsManager(IRepository<Settings> settingsRepository, BusinessLogicUtility utility,
            IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository)
        {
            _settingsRepository = settingsRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _utility = utility;
        }
        public async Task<IBusinessLogicResult<SettingsViewModel>> AdminGetSettingsForEdit(int getterUserId)
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
                        return new BusinessLogicResult<SettingsViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModel>(succeeded: false, result: null,
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
                    return new BusinessLogicResult<SettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                SettingsViewModel settingsViewModel = await _utility.MapAsync<Settings, SettingsViewModel>(settings);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.Successed));
                return new BusinessLogicResult<SettingsViewModel>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<SettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
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

        public async Task<IBusinessLogicResult<HowItWorksViewModel>> GetHowItWorks()
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
                    return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                HowItWorksViewModel settingsViewModel = await _utility.MapAsync<Settings, HowItWorksViewModel>(settings);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.Successed));
                return new BusinessLogicResult<HowItWorksViewModel>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<HowItWorksViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
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

        public async Task<IBusinessLogicResult<SettingsViewModel>> AdminEditSettings(int editorUserId, SettingsViewModel settingsViewModel)
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
                        return new BusinessLogicResult<SettingsViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModel>(succeeded: false, result: null,
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
                    return new BusinessLogicResult<SettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                await _utility.MapAsync(settingsViewModel, settings);
                try
                {
                    await _settingsRepository.UpdateAsync(settings, true);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult<SettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
                }
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.SettingSuccessfullySaved));
                return new BusinessLogicResult<SettingsViewModel>(succeeded: true, messages: messages, result: settingsViewModel);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult<SettingsViewModel>(succeeded: false, messages: messages, exception: exception, result: null);
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
