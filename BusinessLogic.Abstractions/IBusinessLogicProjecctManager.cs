using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicProjectManager : IDisposable
    {
        Task<IBusinessLogicResult<AddProjectViewModel>> AddProjectAsync(AddProjectViewModel addProjectViewModel, int AdderUserId);
        Task<IBusinessLogicResult<ListResultViewModel<ListProjectViewModel>>> GetProjectsAsync(int getterUserId, int page,
           int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<EditProjectViewModel>> EditProjectAsync(EditProjectViewModel editProjectViewModel, int editorUserId);
        Task<IBusinessLogicResult<EditProjectViewModel>> GetProjectForEditAsync(int projectId , int getterUserId);
        Task<IBusinessLogicResult<ProjectDetailsViewModel>> GetProjectDetailsAsync(int projectId);
        Task<IBusinessLogicResult<EditProjectViewModel>> DeleteProjectAsync(int projectId, int deleterUserId);
        Task<IBusinessLogicResult<AcceptOfferViewModel>> AcceptOffer(AcceptOfferViewModel acceptOfferViewModel, int merchantUserId);
        Task<IBusinessLogicResult> DeleteAccept (int acceptId, int merchantUserId);
        Task<IBusinessLogicResult> DeactivateProjectAsync(int projectId, int deactivatorUsertId);
        Task<IBusinessLogicResult> ActivateProjectAsync(int projectId, int activatorUserId);
    }
}
