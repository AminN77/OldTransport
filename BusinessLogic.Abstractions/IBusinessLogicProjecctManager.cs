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

        Task<IBusinessLogicResult<EditProjectViewModel>> DeleteProjectAsync(int projectId,int deleterId);

        Task<IBusinessLogicResult<AcceptOfferViewModel>> AcceptOffer(AcceptOfferViewModel acceptOfferViewModel, int merchantId);
    }
}
