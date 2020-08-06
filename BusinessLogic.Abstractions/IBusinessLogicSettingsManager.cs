using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicSettingsManager: IDisposable
    {
        Task<IBusinessLogicResult<SettingsViewModels>> AdminGetSettingsForEdit(int getterUserId);
        Task<IBusinessLogicResult<IndexSettingsViewModel>> GetIndexSettings();
        Task<IBusinessLogicResult<EditHowItWorksViewModel>> AdminGetHowItWorksForEditAsync(int getterUserId, int howItWorksId);
        Task<IBusinessLogicResult<HowItWorksViewModel>> AdminDeleteHowItWorksAsync(int deleterUserId, int howItWorksId);
        Task<IBusinessLogicResult<TermsAndConditionsViewModel>> GetTermsAndConditions();
        Task<IBusinessLogicResult<SettingsViewModels>> AdminEditSettings(int editorUserId, SettingsViewModels settingsViewModel);
        Task<IBusinessLogicResult<HowItWorksViewModel>> AdminAddHowItWorksAsync(int adderUserId, HowItWorksViewModel howItWorksViewModel);
        Task<IBusinessLogicResult<EditHowItWorksViewModel>> AdminEditHowItWorksAsync(int editorUserId, EditHowItWorksViewModel editHowItWorksViewModel);
        Task<IBusinessLogicResult<ListResultViewModel<ListHowItWorksViewModel>>> ListHowItWorksAsync();
    }
}
