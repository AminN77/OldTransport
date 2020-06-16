using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicOfferManager : IDisposable
    {
        Task<IBusinessLogicResult<AddOfferViewModel>> AddOfferAsync(AddOfferViewModel addProjectViewModel, int AdderUserId);

        Task<IBusinessLogicResult<ListResultViewModel<ListOfferViewModel>>> GetOfferAsync(int getterUserId, int page,
           int pageSize, string search, string sort, string filter);

        Task<IBusinessLogicResult<EditOfferViewModel>> EditOfferAsync(EditOfferViewModel editOfferViewModel, int editorUserId);

        Task<IBusinessLogicResult<EditOfferViewModel>> GetOfferForEditAsync(int offerId, int getterUserId);

        Task<IBusinessLogicResult<EditOfferViewModel>> DeleteOfferAsync(int offerId, int deleterUserId);
    }
}
