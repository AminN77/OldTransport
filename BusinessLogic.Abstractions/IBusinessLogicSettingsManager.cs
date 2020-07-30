﻿using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicSettingsManager: IDisposable
    {
        Task<IBusinessLogicResult<SettingsViewModels>> AdminGetSettingsForEdit(int getterUserId);
        Task<IBusinessLogicResult<IndexSettingsViewModel>> GetIndexSettings();
        Task<IBusinessLogicResult<HowItWorksViewModel>> GetHowItWorks();
        Task<IBusinessLogicResult<TermsAndConditionsViewModel>> GetTermsAndConditions();
        Task<IBusinessLogicResult<SettingsViewModels>> AdminEditSettings(int editorUserId, SettingsViewModels settingsViewModel);
        Task<IBusinessLogicResult<HowItWorksViewModel>> AdminAddHowItWorksAsync(int adderUserId, HowItWorksViewModel howItWorksViewModel);
    }
}
