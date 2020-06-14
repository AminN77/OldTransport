using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicOfferManager : IDisposable
    {
        Task<IBusinessLogicResult<AddOfferViewModel>> AddOfferAsync(AddOfferViewModel addProjectViewModel, int adderId, int projectId);

        Task<IBusinessLogicResult<ListResultViewModel<ListOfferViewModel>>> GetOfferAsync(int getterUserId, int page,
           int pageSize, string search, string sort, string filter);

        Task<IBusinessLogicResult<EditOfferViewModel>> EditOfferAsync(EditOfferViewModel editOfferViewModel, int EditorId);

        Task<IBusinessLogicResult<EditOfferViewModel>> GetOfferForEditAsync(int transporterId, int projectId, int gettetId);

        Task<IBusinessLogicResult<EditOfferViewModel>> DeleteOfferAsync(DeleteOfferViewModel deleteOfferViewModel);

    }
}
