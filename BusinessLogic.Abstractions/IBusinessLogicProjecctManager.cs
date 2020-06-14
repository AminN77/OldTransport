using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicProjectManager : IDisposable
    {
        Task<IBusinessLogicResult<AddProjectViewModel>> AddProjectAsync(AddProjectViewModel addProjectViewModel, int AdderId);

        Task<IBusinessLogicResult<ListResultViewModel<ListProjectViewModel>>> GetProjectsAsync(int getterUserId, int page,
           int pageSize, string search, string sort, string filter);

        Task<IBusinessLogicResult<EditProjectViewModel>> EditProjectAsync(EditProjectViewModel editProjectViewModel, int EditorId);

        Task<IBusinessLogicResult<EditProjectViewModel>> GetProjectForEditAsync(int projectId , int editorId);

        Task<IBusinessLogicResult<EditProjectViewModel>> DeleteProjectAsync(int projectId,int deleterId);

    }
}
