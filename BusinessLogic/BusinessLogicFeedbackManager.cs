﻿using AutoMapper;
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
    public class BusinessLogicFeedbackManager: IBusinessLoginFeedbackManager
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Feedback> _feedbackRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicFeedbackManager(IRepository<User> userRepository, IRepository<Feedback> feedbackRepository,
            BusinessLogicUtility utility, IRepository<UserRole> userRoleRepository, IRepository<Role> roleRepository)
        {
            _feedbackRepository = feedbackRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
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

        public async Task<IBusinessLogicResult<AdminCheckFeedbackViewModel>> GetFeedbackDetailsAsync(int feedbackId, int getterUserId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<AdminCheckFeedbackViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AdminCheckFeedbackViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Feedback feedback;
                try
                {
                    feedback = await _feedbackRepository.FindAsync(feedbackId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AdminCheckFeedbackViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (feedback == null)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.EntityDoesNotExist));
                    return new BusinessLogicResult<AdminCheckFeedbackViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                var adminCheckFeedbackViewModel = await _utility.MapAsync<Feedback,AdminCheckFeedbackViewModel>(feedback);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, message: MessageId.TransporterExists));
                return new BusinessLogicResult<AdminCheckFeedbackViewModel>(succeeded: true, messages: messages, result: adminCheckFeedbackViewModel);

            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<AdminCheckFeedbackViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult<ListResultViewModel<FeedbackListViewModel>>> GetFeedbacksAsync(int getterUserId,
            int page = 1,
            int pageSize = BusinessLogicSetting.MediumDefaultPageSize, string search = null,
            string sort = nameof(ListTransporterViewModel.Name) + ":Asc", string filter = null)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                // Critical Authentication and Authorization
                try
                {
                    var userRole = await _roleRepository.DeferredSelectAll().SingleOrDefaultAsync(role => role.Name == RoleTypes.User.ToString());
                    var isUserAuthorized = _userRoleRepository.DeferredSelectAll().Any(u => u.UserId == getterUserId && u.RoleId != userRole.Id);
                    if (!isUserAuthorized)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.AccessDenied));
                        return new BusinessLogicResult<ListResultViewModel<FeedbackListViewModel>> (succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<ListResultViewModel<FeedbackListViewModel>> (succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                var feedbackQuery = _feedbackRepository.DeferredSelectAll().ProjectTo<FeedbackListViewModel>(new MapperConfiguration(config =>
                    config.CreateMap<Feedback, FeedbackListViewModel>()));

                if (!string.IsNullOrEmpty(search))
                {
                    feedbackQuery = feedbackQuery.Where(user =>
                        user.Name.Contains(search) || user.EmailAddress.Contains(search));
                }

                if (!string.IsNullOrWhiteSpace(filter))
                {
                    feedbackQuery = feedbackQuery.ApplyFilter(filter);
                }

                if (string.IsNullOrWhiteSpace(sort))
                {
                    sort = nameof(FeedbackListViewModel.Name) + ":Asc";
                }
                else
                {
                    var propertyName = sort.Split(':')[0];
                    var propertyInfo = typeof(ListTransporterViewModel).GetProperties().SingleOrDefault(p =>
                        p.Name.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase));
                    if (propertyInfo == null) sort = nameof(FeedbackListViewModel.Name) + ":Asc";
                }

                feedbackQuery = feedbackQuery.ApplyOrderBy(sort);
                var feedbackListViewModel = await feedbackQuery.PaginateAsync(page, pageSize);
                var recordsCount = await feedbackQuery.CountAsync();
                var pageCount = (int)Math.Ceiling(recordsCount / (double)pageSize);
                var result = new ListResultViewModel<FeedbackListViewModel>
                {
                    Results = feedbackListViewModel,
                    Page = page,
                    PageSize = pageSize,
                    TotalEntitiesCount = recordsCount,
                    TotalPagesCount = pageCount
                };
                return new BusinessLogicResult<ListResultViewModel<FeedbackListViewModel>>(succeeded: true,
                    result: result, messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListResultViewModel<FeedbackListViewModel>>(succeeded: false,
                    result: null, messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            _userRepository.Dispose();
            _feedbackRepository.Dispose();
            _userRepository.Dispose();
            _userRoleRepository.Dispose();
        }
    }
}
