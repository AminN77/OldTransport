using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicSettingsManager: IDisposable
    {
        Task<IBusinessLogicResult<SettingsViewModel>> AdminGetSettingsForEdit();
        Task<IBusinessLogicResult<IndexSettingsViewModel>> AdminGetIndexSettings();
        Task<IBusinessLogicResult<HowItWorksViewModel>> AdminGetHowItWorks();
        Task<IBusinessLogicResult<TermsAndConditionsViewModel>> AdminGetTermsAndConditions();
        Task<IBusinessLogicResult<SettingsViewModel>> AdminEditSettings(SettingsViewModel settingsViewModel);
    }
}
