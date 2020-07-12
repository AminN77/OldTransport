using System;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLoginFeedbackManager : IDisposable
    {
        Task<IBusinessLogicResult> SendFeedback(int userId, SendFeedbackViewModel sendFeedbackViewModel);
        Task<IBusinessLogicResult<AdminCheckFeedbackViewModel>> GetFeedbackDetailsAsync(int feedbackId, int getterUserId);
    }
}
