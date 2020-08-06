using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicProjectManager : IDisposable
    {
        Task<IBusinessLogicResult<AddProjectViewModel>> AddProjectAsync(AddProjectViewModel addProjectViewModel, int AdderUserId);
        Task<IBusinessLogicResult<ListResultViewModel<ListProjectViewModel>>> GetProjectsAsync(int page,
            int pageSize, string search, string sort, string filter, int? transporterId);
        Task<IBusinessLogicResult<ListResultViewModel<ListProjectViewModel>>> GetMerchantProjectsAsync(int page,
            int pageSize, string search, string sort, string filter, int getterUserId);
        Task<IBusinessLogicResult<EditProjectViewModel>> EditProjectAsync(EditProjectViewModel editProjectViewModel, int editorUserId);
        Task<IBusinessLogicResult<EditProjectViewModel>> GetProjectForEditAsync(int projectId , int getterUserId);
        Task<IBusinessLogicResult<ProjectDetailsViewModel>> GetProjectDetailsAsync(int projectId, int getterUserId);
        Task<IBusinessLogicResult<DeleteProjectViewModel>> DeleteProjectAsync(int projectId, int deleterUserId);
        Task<IBusinessLogicResult<AcceptOfferViewModel>> AcceptOffer(AcceptOfferViewModel acceptOfferViewModel, int merchantUserId);
        Task<IBusinessLogicResult<AcceptOfferViewModel>> DeleteAccept (int acceptId, int deleterUserId);
        Task<IBusinessLogicResult> DeactivateProjectAsync(int projectId, int deactivatorUsertId);
        Task<IBusinessLogicResult> ActivateProjectAsync(int projectId, int activatorUserId);
        Task<IBusinessLogicResult<int?>> CountProjectsAsync();
        Task<IBusinessLogicResult<ListResultViewModel<CountriesViewModel>>> GetCountriesAsync();
        Task<IBusinessLogicResult<ListResultViewModel<CitiesViewModel>>> GetCitiesAsync(int countryId);
    }
}
