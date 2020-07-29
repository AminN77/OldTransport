using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicOfferManager : IDisposable
    {
        Task<IBusinessLogicResult<AddOfferViewModel>> AddOfferAsync(AddOfferViewModel addProjectViewModel, int AdderUserId);
        Task<IBusinessLogicResult<ListResultViewModel<ListOfferViewModel>>> GetOffersAsync(int page,
           int pageSize, string search, string sort, string filter);

        Task<IBusinessLogicResult<ListResultViewModel<ListOfferViewModel>>> GetTransporterOffersAsync(int page,
            int pageSize, string search, string sort, string filter, int getterUserId);
        Task<IBusinessLogicResult<EditOfferViewModel>> EditOfferAsync(EditOfferViewModel editOfferViewModel, int editorUserId);
        Task<IBusinessLogicResult<EditOfferViewModel>> GetOfferForEditAsync(int offerId, int getterUserId);
        Task<IBusinessLogicResult<DeleteOfferViewModel>> DeleteOfferAsync(int offerId, int deleterUserId);
        Task<IBusinessLogicResult<OfferDetailsViewModel>> GetOfferDetailsAsync(int offerId);
    }
}
