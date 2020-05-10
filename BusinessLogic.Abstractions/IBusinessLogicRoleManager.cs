using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic.Abstractions
{
    public interface IBusinessLogicRoleManager
    {
        Task<IBusinessLogicResult<IList<ListRoleViewModel>>> FindUserRolesAsync(int userId);
        Task<IBusinessLogicResult<bool>> IsUserInRoleAsync(int userId, string roleName);
        Task<IBusinessLogicResult<IList<ListUserViewModel>>> FindUsersInRoleAsync(string roleName);
    }
}
