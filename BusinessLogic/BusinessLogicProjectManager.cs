using AutoMapper;
using AutoMapper.QueryableExtensions;
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
    public class BusinessLogicProjectManager : IBusinessLogicProjectManager
    {
        private readonly IRepository<Merchant> _merchantRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicProjectManager(IRepository<Merchant> merchantRepository, IRepository<Project> projectRepository, BusinessLogicUtility utility)
        {
            _merchantRepository = merchantRepository;
            _projectRepository = projectRepository;
            _utility = utility;
        }

        public async Task<IBusinessLogicResult<AddProjectViewModel>> AddProjectAsync(AddProjectViewModel addProjectViewModel, int AdderId)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                Merchant merchant;

                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == AdderId);

                    if (merchant == null)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied));
                        return new BusinessLogicResult<AddProjectViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }

                }

                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Project project = new Project()
                {
                    Beginning = addProjectViewModel.Beginning,
                    Description = addProjectViewModel.Description,
                    Destination = addProjectViewModel.Destination,
                    Title = addProjectViewModel.Title,
                    Budget = addProjectViewModel.Budget,
                    Weight = addProjectViewModel.Weight,

                    CreateDateTime = DateTime.Now,
                    IsDeleted = false,
                    MerchantId = merchant.Id,
                    Merchant = merchant
                };


                try
                {
                    await _projectRepository.AddAsync(project);

                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.ProjectSuccessfullyAdded));
                    return new BusinessLogicResult<AddProjectViewModel>(succeeded: true, result: addProjectViewModel,
                        messages: messages);
                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

            }

            catch (Exception exception)
            {

                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<AddProjectViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<EditProjectViewModel>> DeleteProjectAsync(DeleteProjectViewModel deleteProjectViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                //// Critical Authentication and Authorization
                //var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(getterUserId,
                //    UserManagerPermissions.EditUser.ToString());
                //if (!isUserInPermission.Succeeded) return isUserInPermission;

                Project project;
                // Critical Database
                try
                {
                    project = await _projectRepository.FindAsync(deleteProjectViewModel.Id);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User Verification
                if (project == null || project.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                project.IsDeleted = true;

                try
                {
                    await _projectRepository.UpdateAsync(project, true);
                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                return new BusinessLogicResult<EditProjectViewModel>(succeeded: true, result: null,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        

        public async Task<IBusinessLogicResult<EditProjectViewModel>> EditProjectAsync(EditProjectViewModel editProjectViewModel, int EditorId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                //                // Critical Database
                //
                //                var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(editorUserId,
                //                    UserManagerPermissions.EditUser.ToString());
                //                if (!isUserInPermission.Succeeded) return isUserInPermission;

                // Check organization level
                //                var getterUser = await _userRepository.FindAsync(editorUserId);
                //                var subLevelsId = await GetSubLevels(getterUser.OrganizationLevelId);
                //                if (!(subLevelsId.Result.Contains(editUserViewModel.OrganizationLevelId) ||
                //                      editUserViewModel.Id == editorUserId))
                //                {
                //                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                //                        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                //                    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                //                        messages: messages);
                //                }

                // User verification
                //var isUserNameExisted = await _projectRepository.DeferredSelectAll(u => u.Id != editUserViewModel.Id)
                //  .AnyAsync(usr => usr.EmailAddress == editUserViewModel.EmailAddress);

                Project project;

                try
                {
                    project = await _projectRepository.DeferredSelectAll().SingleOrDefaultAsync(p => p.Id == editProjectViewModel.Id);

                    if (project == null || project.IsDeleted)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                                message: MessageId.ProjectNotFound, BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                           messages: messages);
                    }

                    if (project.MerchantId != EditorId)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                                message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                           messages: messages);
                    }

                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);

                }

                try
                {
                    project.Beginning = editProjectViewModel.Beginning;
                    project.Destination = editProjectViewModel.Destination;
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                ////Check developer role
                //var developerRole = await _userRoleRepository.DeferredSelectAll()
                //    .SingleOrDefaultAsync(role => role.RoleId == Int32.Parse(RoleTypes.DeveloperSupport.ToString()));
                //if (editUserViewModel.RoleIds.Contains(developerRole.Id))
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                //        message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                //    return new BusinessLogicResult<AddUserViewModel>(succeeded: false, result: null,
                //        messages: messages);
                //}

                // Check Username Existed
                //if (isUserNameExisted)
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                //        message: MessageId.UsernameAlreadyExisted,
                //        viewMessagePlaceHolders: editUserViewModel.EmailAddress));
                //    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                //        messages: messages);
                //}

                //User user;

                //try
                //{
                //    user = await _userRepository.FindAsync(editUserViewModel.Id);
                //}
                //catch (Exception exception)
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                //    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                //        messages: messages, exception: exception);
                //}

                //if (user == null || user.IsDeleted)
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                //        message: MessageId.EntityDoesNotExist,
                //        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
                //    return new BusinessLogicResult<EditUserViewModel>(succeeded: false, result: null,
                //        messages: messages);
                //}

                //// Safe
                //user = await _utility.MapAsync(editUserViewModel, user);
                //var userId = user.Id;
                //var oldUserRoles = await _userRoleRepository.DeferredWhere(userRole => userRole.UserId == userId)
                //    .ToListAsync();
                //var userOldRoleIds = oldUserRoles.Select(userRole => userRole.RoleId).ToList();
                //if (editUserViewModel.RoleIds != null)
                //{
                //    var toBeDeletedUserRoleIds = userOldRoleIds.Except(editUserViewModel.RoleIds);
                //    var toBeDeletedUserRoles = oldUserRoles
                //        .Where(userRole => toBeDeletedUserRoleIds.Contains(userRole.RoleId)).ToList();
                //    await _userRoleRepository.DeleteAllAsync(toBeDeletedUserRoles, false);
                //    var toBeAddedUserRoleIds = editUserViewModel.RoleIds.Except(userOldRoleIds);
                //    foreach (var roleId in toBeAddedUserRoleIds)
                //    {
                //        var userRole = new UserRole
                //        {
                //            RoleId = roleId,
                //            UserId = user.Id
                //        };
                //        await _userRoleRepository.AddOrUpdateAsync(userRole, false);
                //    }
                //}

                // Set new serial number
                //user.SerialNumber = Guid.NewGuid().ToString();
                //user.Name = editUserViewModel.Name;
                //user.Picture = editUserViewModel.Picture;

                try
                {
                    await _projectRepository.UpdateAsync(project, true); //, propertiesToBeUpdate.ToArray()
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                messages.Add(
                    new BusinessLogicMessage(type: MessageType.Info, message: MessageId.UserSuccessfullyEdited));
                return new BusinessLogicResult<EditProjectViewModel>(succeeded: true, result: editProjectViewModel,
                    messages: messages);
            }

            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<EditProjectViewModel>> GetProjectForEditAsync(int projectId)
        {

            var messages = new List<IBusinessLogicMessage>();
            try
            {
                //// Critical Authentication and Authorization
                //var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(getterUserId,
                //    UserManagerPermissions.EditUser.ToString());
                //if (!isUserInPermission.Succeeded) return isUserInPermission;

                Project project;
                // Critical Database
                try
                {
                    project = await _projectRepository.FindAsync(projectId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User Verification
                if (project == null || project.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                // Safe Map
                var editViewModel = await _utility.MapAsync<Project, EditProjectViewModel>(project);
                //if (userId != getterUserId)
                //    userViewModel.RoleIds = await _userRoleRepository
                //        .DeferredWhere(userRole => userRole.UserId == userId).Select(userRole => userRole.RoleId)
                //        .ToArrayAsync();
                return new BusinessLogicResult<EditProjectViewModel>(succeeded: true, result: editViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<ListResultViewModel<ListProjectViewModel>>> GetProjectsAsync(int getterUserId, int page, int pageSize, string search, string sort, string filter)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                var getterUser = await _projectRepository.FindAsync(getterUserId);
                // Solution 1:
                // Todo: Abolfazl -> set developer role type. (Abolfazl)
                const bool developerUser = true; // RoleType.DeveloperSupport;
                var usersQuery = _projectRepository.DeferredWhere(u =>
                        (!u.Merchant.User.IsDeleted && !developerUser) || developerUser
                    )
                    .ProjectTo<ListProjectViewModel>(new MapperConfiguration(config =>
                        config.CreateMap<Project, ListProjectViewModel>()));
                if (!string.IsNullOrEmpty(search))
                {
                    usersQuery = usersQuery.Where(project =>
                        project.Beginning.Contains(search) || project.Destination.Contains(search));

                }

                //TODO : isDeleted must add

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    usersQuery = usersQuery.ApplyFilter(filter);
                }

                if (string.IsNullOrWhiteSpace(sort))
                {
                    sort = nameof(ListProjectViewModel.Beginning) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListProjectViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(ListProjectViewModel.Beginning) + ":Asc";
                }

                usersQuery = usersQuery.ApplyOrderBy(sort);
                var projectListViewModels = await usersQuery.PaginateAsync(page, pageSize);
                var recordsCount = await usersQuery.CountAsync();
                var pageCount = (int)Math.Ceiling(recordsCount / (double)pageSize);
                var result = new ListResultViewModel<ListProjectViewModel>
                {
                    Results = projectListViewModels,
                    Page = page,
                    PageSize = pageSize,
                    TotalEntitiesCount = recordsCount,
                    TotalPagesCount = pageCount
                };
                return new BusinessLogicResult<ListResultViewModel<ListProjectViewModel>>(succeeded: true,
                    result: result, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<ListProjectViewModel>>(succeeded: false,
                    result: null, messages: messages, exception: exception);
            }

        }

        public void Dispose()
        {
            _projectRepository.Dispose();
            _merchantRepository.Dispose();
        }
    }
}
