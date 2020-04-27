using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IUserAuthenticator
    {
        Task<IBusinessLogicResult<UserSignInInfoViewModel>> IsUserAuthenticateAsync(SignInInfoViewModel signInInfoViewModel);
    }
}