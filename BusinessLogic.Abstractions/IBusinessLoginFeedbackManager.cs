using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLoginFeedbackManager : IDisposable
    {
        Task<IBusinessLogicResult> SendFeedback(int userId, SendFeedbackViewModel sendFeedbackViewModel);
        Task<IBusinessLogicResult<AdminCheckFeedbackViewModel>> GetFeedbackDetailsAsync(int feedbackId, int getterUserId);
        Task<IBusinessLogicResult<ListResultViewModel<FeedbackListViewModel>>> GetFeedbacksAsync(int getterUserId, int page,
             int pageSize, string search, string sort, string filter);
    }
}
