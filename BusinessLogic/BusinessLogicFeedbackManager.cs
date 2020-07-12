using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Data.Abstractions;
using Data.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic
{
    public class BusinessLogicFeedbackManager: IBusinessLoginFeedbackManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Feedback> _feedbackRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicFeedbackManager(IRepository<User> userRepository, IRepository<Feedback> feedbackRepository,
            BusinessLogicUtility utility)
        {
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
            _utility = utility;
        }
        public async Task<IBusinessLogicResult> SendFeedback(int userId, SendFeedbackViewModel sendFeedbackViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                User user;
                try
                {
                    user = await _userRepository.FindAsync(userId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult(succeeded: false,
                        messages: messages, exception: exception);
                }

                var feedback = await _utility.MapAsync<User, Feedback>(user);
                feedback.Text = sendFeedbackViewModel.Text;
                feedback.CreateDateTime = DateTime.Now;

                try
                {
                    await _feedbackRepository.AddAsync(feedback);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false,
                        messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.TransporterExists));
                return new BusinessLogicResult(succeeded: true, messages: messages);

            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false,
                    messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            _userRepository.Dispose();
            _feedbackRepository.Dispose();
        }
    }
}
