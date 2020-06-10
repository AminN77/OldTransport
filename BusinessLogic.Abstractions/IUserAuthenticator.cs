using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IUserAuthenticator
    {
        Task<IBusinessLogicResult<UserSignInViewModel>> IsUserAuthenticateAsync(SignInInfoViewModel signInInfoViewModel);
        Task<IBusinessLogicResult> DoesEmailExistAsync(EmailViewModel emailViewModel);
    }
}