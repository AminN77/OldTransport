using AutoMapper;
using AutoMapper.QueryableExtensions;
using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Cross.Abstractions.EntityEnums;
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
        private readonly IRepository<Accept> _acceptRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicProjectManager(IRepository<Merchant> merchantRepository, IRepository<Project> projectRepository,
                BusinessLogicUtility utility, IRepository<Accept> acceptRepository, IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository)
        {
            _merchantRepository = merchantRepository;
            _projectRepository = projectRepository;
            _acceptRepository = acceptRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _utility = utility;
        }

        public async Task<IBusinessLogicResult<AddProjectViewModel>> AddProjectAsync(AddProjectViewModel addProjectViewModel, int AdderUserId)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == AdderUserId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
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

                Merchant merchant;

                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == AdderUserId);
                }

                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Project project;        

                try
                {
                   project = await _utility.MapAsync<AddProjectViewModel, Project>(addProjectViewModel);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                project.CreateDateTime = DateTime.Now;
                project.IsEnabled = true;
                project.MerchantId = merchant.Id;
                project.Merchant = merchant;

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

        public async Task<IBusinessLogicResult<EditProjectViewModel>> DeleteProjectAsync(int projectId,int deleterUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deleterUserId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
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
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                if (project.Merchant.UserId != deleterUserId)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                try
                {
                    await _projectRepository.DeleteAsync(project, true);
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

        public async Task<IBusinessLogicResult<EditProjectViewModel>> EditProjectAsync(EditProjectViewModel editProjectViewModel, int editorUserId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == editorUserId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
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

                Project project;

                try
                {
                    project = await _projectRepository.DeferredSelectAll().SingleOrDefaultAsync(p => p.Id == editProjectViewModel.Id);

                    if (project == null)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                                message: MessageId.ProjectNotFound, BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                           messages: messages);
                    }

                    if (project.Merchant.UserId != editorUserId)
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
                    await _utility.MapAsync<EditProjectViewModel, Project>(editProjectViewModel);
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

        public async Task<IBusinessLogicResult<EditProjectViewModel>> GetProjectForEditAsync(int projectId,int getterUserId)
        {

            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
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
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                if(project.Merchant.UserId != getterUserId)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied,
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
                        project.BeginningCountry.Contains(search) || project.DestinationCountry.Contains(search) || project.DestinationCity.Contains(search)
                        || project.BeginningCity.Contains(search));

                }

                //TODO : isDeleted must add

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    usersQuery = usersQuery.ApplyFilter(filter);
                }

                if (string.IsNullOrWhiteSpace(sort))
                {
                    sort = nameof(ListProjectViewModel.BeginningCountry) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListProjectViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(ListProjectViewModel.BeginningCountry) + ":Asc";
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

        public async Task<IBusinessLogicResult<AcceptOfferViewModel>> AcceptOffer(AcceptOfferViewModel acceptOfferViewModel, int merchantId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == merchantId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<AcceptOfferViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AcceptOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                acceptOfferViewModel.MerchantId = merchantId;
                var accept = await _utility.MapAsync<AcceptOfferViewModel, Accept>(acceptOfferViewModel);
                try
                {
                    await _acceptRepository.AddAsync(accept);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AcceptOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.EntitySuccessfullyAdded));
                return new BusinessLogicResult<AcceptOfferViewModel>(succeeded: false, result: acceptOfferViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<AcceptOfferViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> DeactivateProjectAsync(int projectId, int deactivatorUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.Admin.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == deactivatorUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Project project;
                // Critical Database
                try
                {
                    project = await _projectRepository.FindAsync(projectId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // project Verification
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.EntityDoesNotExist,
                        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                project.IsEnabled = false;
                try
                {
                    await _projectRepository.UpdateAsync(project);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.UserSuccessfullyDeactivated));
                return new BusinessLogicResult(succeeded: true, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> ActivateProjectAsync(int projectId, int activatorUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.Admin.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == activatorUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Project project;
                // Critical Database
                try
                {
                    project = await _projectRepository.FindAsync(projectId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // project Verification
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                        message: MessageId.EntityDoesNotExist,
                        viewMessagePlaceHolders: BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<DetailUserViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                project.IsEnabled = true;
                try
                {
                    await _projectRepository.UpdateAsync(project);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
                messages.Add(new BusinessLogicMessage(MessageType.Info, MessageId.UserSuccessfullyDeactivated));
                return new BusinessLogicResult(succeeded: true, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Critical, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            _projectRepository.Dispose();
            _merchantRepository.Dispose();
        }
    }
}
