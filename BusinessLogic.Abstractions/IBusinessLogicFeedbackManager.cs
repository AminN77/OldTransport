using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicFeedbackManager : IDisposable
    {
        Task<IBusinessLogicResult> SendFeedback(int userId, SendFeedbackViewModel sendFeedbackViewModel);
        Task<IBusinessLogicResult<AdminCheckFeedbackViewModel>> GetFeedbackDetailsAsync(int feedbackId, int getterUserId);
        Task<IBusinessLogicResult<ListResultViewModel<FeedbackListViewModel>>> GetFeedbacksAsync(int getterUserId, int page,
             int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult> ContactUsAsync(ContactUsViewModel contactUsViewModel);
        Task<IBusinessLogicResult<ListResultViewModel<ContactMessagesListViewModel>>> GetContactMessagesAsync(int getterUserId, int page,
            int pageSize, string search, string sort, string filter);
        Task<IBusinessLogicResult<ContactMessageViewModel>> GetContactMessageAsync(int messageId, int getterUserId);
    }
}
