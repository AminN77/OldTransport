﻿using AutoMapper;
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
    public class BusinessLogicOfferManager : IBusinessLogicOfferManager
    {

        private readonly IRepository<Transporter> _transporterRepository;
        private readonly IRepository<Offer> _offerRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicOfferManager(IRepository<Offer> offerRepository, IRepository<Transporter> transporterRepository
            , BusinessLogicUtility utility, IRepository<Project> projectRepository)
        {
            _transporterRepository = transporterRepository;
            _offerRepository = offerRepository;
            _projectRepository = projectRepository;
            _utility = utility;
        }

        public async Task<IBusinessLogicResult<AddOfferViewModel>> AddOfferAsync(AddOfferViewModel addOfferViewModel, int adderId)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                Transporter transporter;
                Project project;
                Offer DuplicateOffer;

                try
                {
                    transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == adderId);

                }

                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (transporter == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                try
                {
                    project = await _projectRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.Id == addOfferViewModel.ProjectId);

                }

                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (project == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                try
                {
                    DuplicateOffer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(
                        o => o.ProjectId == addOfferViewModel.ProjectId && o.TransporterId == addOfferViewModel.TransporterId);
                }

                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (DuplicateOffer != null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }


                Offer offer;

                try
                {
                    offer = await _utility.MapAsync<AddOfferViewModel, Offer>(addOfferViewModel);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                offer.CreateDate = DateTime.Now;
                offer.IsDeleted = false;
                offer.Transporter = transporter;
                offer.Project = project;

                try
                {
                    await _offerRepository.AddAsync(offer);

                    messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.ProjectSuccessfullyAdded));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: true, result: addOfferViewModel,
                        messages: messages);
                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

            }

            catch (Exception exception)
            {

                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<ListResultViewModel<ListOfferViewModel>>> GetOfferAsync(int getterUserId,
            int page, int pageSize, string search, string sort, string filter)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                var getterUser = await _offerRepository.FindAsync(getterUserId);
                // Solution 1:
                // Todo: Abolfazl -> set developer role type. (Abolfazl)
                const bool developerUser = true; // RoleType.DeveloperSupport;
                var usersQuery = _offerRepository.DeferredWhere(u =>
                        (!u.Transporter.User.IsDeleted && !developerUser) || developerUser
                    )
                    .ProjectTo<ListOfferViewModel>(new MapperConfiguration(config =>
                        config.CreateMap<Offer, ListOfferViewModel>()));
                if (!string.IsNullOrEmpty(search))
                {
                    usersQuery = usersQuery.Where(offer =>
                        offer.Description.Contains(search));

                }

                //TODO : isDeleted must add

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    usersQuery = usersQuery.ApplyFilter(filter);
                }

                if (string.IsNullOrWhiteSpace(sort))
                {
                    sort = nameof(ListOfferViewModel.Price) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListProjectViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(ListOfferViewModel.Price) + ":Asc";
                }

                usersQuery = usersQuery.ApplyOrderBy(sort);
                var offerListViewModels = await usersQuery.PaginateAsync(page, pageSize);
                var recordsCount = await usersQuery.CountAsync();
                var pageCount = (int)Math.Ceiling(recordsCount / (double)pageSize);
                var result = new ListResultViewModel<ListOfferViewModel>
                {
                    Results = offerListViewModels,
                    Page = page,
                    PageSize = pageSize,
                    TotalEntitiesCount = recordsCount,
                    TotalPagesCount = pageCount
                };
                return new BusinessLogicResult<ListResultViewModel<ListOfferViewModel>>(succeeded: true,
                    result: result, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<ListOfferViewModel>>(succeeded: false,
                    result: null, messages: messages, exception: exception);
            }

        }


        public async Task<IBusinessLogicResult<EditProjectViewModel>> GetOfferForEditAsync(int transporterId,int projectId , int getterId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                //// Critical Authentication and Authorization
                //var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(getterUserId,
                //    UserManagerPermissions.EditUser.ToString());
                //if (!isUserInPermission.Succeeded) return isUserInPermission;

                Project project;
                Transporter transporter;
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

                try
                {
                    transporter = await _transporterRepository.FindAsync(transporterId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User Verification
                if (transporter == null) { 
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditProjectViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                if (transporter.UserId != getterId)
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

        public async Task<IBusinessLogicResult<EditProjectViewModel>> DeleteOfferAsync(DeleteProjectViewModel deleteProjectViewModel)
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


        public async Task<IBusinessLogicResult<EditOfferViewModel>> EditOfferAsync(EditOfferViewModel editOfferViewModel, int EditorId)
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

                Offer offer;

                try
                {
                    try
                    {
                        offer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(
                       o => o.TransporterId == editOfferViewModel.TransporterId || o.ProjectId == editOfferViewModel.ProjectId);

                    }
                    catch (Exception exception)
                    {

                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                        return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                            messages: messages, exception: exception);

                    }

                    if (offer == null || offer.IsDeleted)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                                message: MessageId.ProjectNotFound, BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                           messages: messages);
                    }

                    if (offer.TransporterId != EditorId)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                                message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                           messages: messages);
                    }

                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);

                }

                try
                {
                    offer = await _utility.MapAsync<EditOfferViewModel, Offer>(editOfferViewModel);

                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
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
                    await _offerRepository.UpdateAsync(offer, true); //, propertiesToBeUpdate.ToArray()
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                messages.Add(
                    new BusinessLogicMessage(type: MessageType.Info, message: MessageId.UserSuccessfullyEdited));
                return new BusinessLogicResult<EditOfferViewModel>(succeeded: true, result: editOfferViewModel,
                    messages: messages);
            }

            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<EditOfferViewModel>> GetOfferForEditAsync(int transporterId, int projectId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                //// Critical Authentication and Authorization
                //var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(getterUserId,
                //    UserManagerPermissions.EditUser.ToString());
                //if (!isUserInPermission.Succeeded) return isUserInPermission;

                Offer offer;
                // Critical Database
                try
                {
                    offer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(o => o.TransporterId == transporterId && o.ProjectId == projectId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User Verification
                if (offer == null || offer.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                // Safe Map
                var editViewModel = await _utility.MapAsync<Offer, EditOfferViewModel>(offer);
                //if (userId != getterUserId)
                //    userViewModel.RoleIds = await _userRoleRepository
                //        .DeferredWhere(userRole => userRole.UserId == userId).Select(userRole => userRole.RoleId)
                //        .ToArrayAsync();
                return new BusinessLogicResult<EditOfferViewModel>(succeeded: true, result: editViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult<EditOfferViewModel>> DeleteOfferAsync(DeleteOfferViewModel deleteOfferViewModel)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                //// Critical Authentication and Authorization
                //var isUserInPermission = await IsUserInPermissionAsync<EditUserViewModel>(getterUserId,
                //    UserManagerPermissions.EditUser.ToString());
                //if (!isUserInPermission.Succeeded) return isUserInPermission;

                Offer offer;
                // Critical Database
                try
                {
                    offer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(o =>
                    o.ProjectId == deleteOfferViewModel.ProjectId && o.TransporterId == deleteOfferViewModel.TransporterId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User Verification
                if (offer == null || offer.IsDeleted)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                offer.IsDeleted = true;

                try
                {
                    await _offerRepository.UpdateAsync(offer, true);
                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                return new BusinessLogicResult<EditOfferViewModel>(succeeded: true, result: null,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public void Dispose()
        {
            _projectRepository.Dispose();
            _transporterRepository.Dispose();
            _offerRepository.Dispose();
        }

    }
}
