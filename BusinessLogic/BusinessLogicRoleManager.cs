using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Data.Abstractions;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic
{
    public class BusinessLogicRoleManager : IBusinessLogicRoleManager
    {
        // Variables
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicRoleManager(IRepository<User> userRepository, IRepository<UserRole> userRoleRepository, IRepository<Role> roleRepository, BusinessLogicUtility utility)
        {
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _utility = utility;
        }

       

        public async Task<IBusinessLogicResult<IList<ListRoleViewModel>>> FindUserRolesAsync(int userId)
        {
            var messages = new List<IBusinessLogicMessage>();
            IList<ListRoleViewModel> roleListViewModels = new List<ListRoleViewModel>();
            try
            {
                roleListViewModels = await _userRepository.DeferredWhere(user => user.Id == userId)
                    .Join(_userRoleRepository.DeferredSelectAll(),
                    user => user.Id,
                    userRole => userRole.UserId,
                    (user, userRole) => userRole)
                    .Join(_roleRepository.DeferredSelectAll(),
                    userRole => userRole.RoleId,
                    role => role.Id,
                    (userRole, role) => new ListRoleViewModel
                    {
                        Id = role.Id,
                        Name = role.Name
                    }).OrderBy(r => r.Name).ToListAsync();

                return new BusinessLogicResult<IList<ListRoleViewModel>>(succeeded: true, result: roleListViewModels,
                messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<IList<ListRoleViewModel>>(succeeded: false, result: roleListViewModels,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<IList<ListUserViewModel>>> FindUsersInRoleAsync(string roleName)
        {
            var messages = new List<IBusinessLogicMessage>();
            IList<ListUserViewModel> usersInRoleListViewModels = new List<ListUserViewModel>();
            try
            {

                usersInRoleListViewModels = await _roleRepository.DeferredWhere(role => role.Name == roleName)
                    .Join(_userRoleRepository.DeferredSelectAll(),
                    role => role.Id,
                    userRole => userRole.RoleId,
                    (role, userRole) => userRole)
                    .Join(_userRepository.DeferredSelectAll(),
                    userRole => userRole.UserId,
                    user => user.Id,
                    (userRole, user) => new ListUserViewModel
                    {
                        Id = user.Id,
                        EmailAddress = user.EmailAddress,
                        IsEnabled = user.IsEnabled,
                        LastLoggedIn = user.LastLoggedIn,
                        Name = user.Name,
                        Picture = user.Picture
                    }).ToListAsync();

                return new BusinessLogicResult<IList<ListUserViewModel>>(succeeded: true, result: usersInRoleListViewModels,
                messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<IList<ListUserViewModel>>(succeeded: false, result: usersInRoleListViewModels,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<bool>> IsUserInRoleAsync(int userId, string roleName)
        {
            var messages = new List<IBusinessLogicMessage>();
            bool isUserInRole = false;
            try
            {
                isUserInRole = await _userRepository.DeferredWhere(user => user.Id == userId && user.IsEnabled)
                   .Join(_userRoleRepository.DeferredSelectAll(),
                   user => user.Id,
                   userRole => userRole.UserId,
                   (role, userRole) => userRole)
                   .Join(_roleRepository.DeferredWhere(role => role.Name == roleName),
                   userRole => userRole.RoleId,
                   role => role.Id,
                   (userRole, role) => role).AnyAsync();

                return new BusinessLogicResult<bool>(succeeded: true, result: isUserInRole,
                messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<bool>(succeeded: false, result: isUserInRole,
                    messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            
            _roleRepository.Dispose();
            _userRoleRepository.Dispose();
            _userRepository.Dispose();
           
        }

    }
}
