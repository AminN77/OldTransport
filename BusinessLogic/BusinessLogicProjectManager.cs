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
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Offer> _offerRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicProjectManager(IRepository<Merchant> merchantRepository, IRepository<Project> projectRepository,
                BusinessLogicUtility utility, IRepository<Accept> acceptRepository, IRepository<Role> roleRepository,
                IRepository<UserRole> userRoleRepository, IRepository<User> userRepository, IRepository<Offer> offerRepository)
        {
            _merchantRepository = merchantRepository;
            _projectRepository = projectRepository;
            _acceptRepository = acceptRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _offerRepository = offerRepository;
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

                // project Verification
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                Merchant merchant;
                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.Id == project.MerchantId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (merchant.UserId != deleterUserId)
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
                var editProjectViewModel = await _utility.MapAsync<Project, EditProjectViewModel>(project);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, MessageId.EntitySuccessfullyDeleted));
                return new BusinessLogicResult<EditProjectViewModel>(succeeded: true, result:editProjectViewModel ,
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
                Merchant merchant;

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

                    try
                    {
                        merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.Id == project.MerchantId);
                    }
                    catch (Exception exception)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                        return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                            messages: messages, exception: exception);
                    }

                    if (merchant.UserId != editorUserId)
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

                project.Title = editProjectViewModel.Title;
                project.Description = editProjectViewModel.Description;
                project.DestinationCity = editProjectViewModel.DestinationCity;
                project.DestinationCountry = editProjectViewModel.DestinationCountry;
                project.BeginningCity = editProjectViewModel.BeginningCity;
                project.BeginningCountry = editProjectViewModel.DestinationCountry;
                project.Budget = editProjectViewModel.Budget;
                project.Weight = editProjectViewModel.Weight;

                //try
                //{
                //    project = await _utility.MapAsync<EditProjectViewModel, Project>(editProjectViewModel);
                //}
                //catch (Exception exception)
                //{
                //    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                //    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                //        messages: messages, exception: exception);
                //}
                //project.MerchantId = merchant.Id;

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

                // Verification
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                Merchant merchant;
                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.Id == project.MerchantId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (merchant.UserId != getterUserId)
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

        public async Task<IBusinessLogicResult<ProjectDetailsViewModel>> GetProjectDetailsAsync(int projectId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                Project project;
                // Critical Database
                try
                {
                    project = await _projectRepository.FindAsync(projectId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<ProjectDetailsViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // Verification
                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<ProjectDetailsViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                var projectDetailsViewModel = await _utility.MapAsync<Project, ProjectDetailsViewModel>(project);
                try
                {
                    var merchant = await _merchantRepository.FindAsync(project.MerchantId);
                    var user = await _userRepository.FindAsync(merchant.UserId);
                    projectDetailsViewModel.MerchantName = user.Name;
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<ProjectDetailsViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                try
                {
                    var count = _offerRepository.DeferredWhere(o => o.ProjectId == project.Id).Count();
                    projectDetailsViewModel.count = count;
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<ProjectDetailsViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                return new BusinessLogicResult<ProjectDetailsViewModel>(succeeded: true, result: projectDetailsViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<ProjectDetailsViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<ListResultViewModel<ListProjectViewModel>>> GetProjectsAsync(int page, int pageSize, string search, string sort, string filter)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                var usersQuery = _projectRepository.DeferredSelectAll()
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
                foreach (var item in projectListViewModels)
                {
                    var acceptedOffer = await _offerRepository.DeferredWhere(o => o.ProjectId == item.Id)
                        .Join(_acceptRepository.DeferredSelectAll(),
                        o => o.Id,
                        a => a.OfferId,
                        (o, a) => o).Distinct().SingleOrDefaultAsync();
                    if(acceptedOffer != null) item.AcceptedOfferId = acceptedOffer.Id;
                }

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

        public async Task<IBusinessLogicResult<AcceptOfferViewModel>> AcceptOffer(AcceptOfferViewModel acceptOfferViewModel, int merchantUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == merchantUserId && u.RoleId == userRole.Id);
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
                Merchant merchant;
                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == merchantUserId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AcceptOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                try
                {
                    var offerProject = await _offerRepository.DeferredWhere(o => o.Id == acceptOfferViewModel.OfferId)
                        .Join(_projectRepository.DeferredSelectAll(),
                        o => o.ProjectId,
                        p => p.Id,
                        (o, p) => p).SingleOrDefaultAsync();
                    if (offerProject.MerchantId != merchant.Id)
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

                acceptOfferViewModel.MerchantId = merchant.Id;
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

        public async Task<IBusinessLogicResult> DeleteAccept(int acceptId, int merchantUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == merchantUserId && u.RoleId == userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }
                Accept accept;
                // Critical Database
                try
                {
                    accept = await _acceptRepository.FindAsync(acceptId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                // Verification
                if (accept == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult(succeeded: false, messages: messages);
                }

                Merchant merchant;
                try
                {
                    merchant = await _merchantRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.Id == accept.MerchantId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                if (merchant.UserId != merchantUserId)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult(succeeded: false, messages: messages);
                }

                try
                {
                    await _acceptRepository.DeleteAsync(accept, true);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info, MessageId.EntitySuccessfullyDeleted));
                return new BusinessLogicResult(succeeded: true, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult(succeeded: false, messages: messages, exception: exception);
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
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
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
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
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
