using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicSettingsManager: IDisposable
    {
        Task<IBusinessLogicResult<SettingsViewModel>> AdminGetSettingsForEdit(int getterUserId);
        Task<IBusinessLogicResult<IndexSettingsViewModel>> GetIndexSettings();
        Task<IBusinessLogicResult<HowItWorksViewModel>> GetHowItWorks();
        Task<IBusinessLogicResult<TermsAndConditionsViewModel>> GetTermsAndConditions();
        Task<IBusinessLogicResult<SettingsViewModel>> AdminEditSettings(int editorUserId, SettingsViewModel settingsViewModel);
    }
}
