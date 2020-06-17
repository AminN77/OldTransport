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
    public class BusinessLogicOfferManager : IBusinessLogicOfferManager
    {

        private readonly IRepository<Transporter> _transporterRepository;
        private readonly IRepository<Offer> _offerRepository;
        private readonly IRepository<Project> _projectRepository;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;
        private readonly IRepository<User> _userRepository;
        private readonly BusinessLogicUtility _utility;

        public BusinessLogicOfferManager(IRepository<Offer> offerRepository, IRepository<Transporter> transporterRepository,
                BusinessLogicUtility utility, IRepository<Project> projectRepository, IRepository<Role> roleRepository,
                IRepository<UserRole> userRoleRepository, IRepository<User> userRepository)
        {
            _transporterRepository = transporterRepository;
            _offerRepository = offerRepository;
            _projectRepository = projectRepository;
            _userRoleRepository = userRoleRepository;
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _utility = utility;
        }

        public async Task<IBusinessLogicResult<AddOfferViewModel>> AddOfferAsync(AddOfferViewModel addOfferViewModel, int AdderUserId)
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
                        return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                            messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                Transporter transporter;
                Project project;
                Offer DuplicateOffer;

                try
                {
                    transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(m => m.UserId == AdderUserId);
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
                        o => o.ProjectId == addOfferViewModel.ProjectId && o.TransporterId == transporter.Id);
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
                    message: MessageId.EntitySuccessfullyAdded));
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

        public async Task<IBusinessLogicResult<ListResultViewModel<ListOfferViewModel>>> GetOfferAsync(
            int page, int pageSize, string search, string sort, string filter)
        {
            var messages = new List<IBusinessLogicMessage>();

            try
            {
                var usersQuery = _offerRepository.DeferredWhere(o => !o.IsDeleted)
                        .Join(_transporterRepository.DeferredSelectAll(),
                        o => o.Id,
                        t => t.Id,
                        (o, t) => new { o, t })
                            .Join(_userRepository.DeferredSelectAll(),
                            c => c.o.Id,
                            u => u.Id,
                            (c, u) => new ListOfferViewModel()
                            {
                                Id = c.o.Id,
                                Description = c.o.Description,
                                TransporterName = u.Name,
                                EstimatedTime = c.o.EstimatedTime,
                                Price = c.o.Price,
                                TransporterId = c.o.TransporterId,
                                ProjectId = c.o.ProjectId
                            });

                //_offerRepository.DeferredWhere(u =>
                //    (!u.IsDeleted)
                //)
                //.ProjectTo<ListOfferViewModel>(new MapperConfiguration(config =>
                //    config.CreateMap<Offer, ListOfferViewModel>().ForMember(o => o.TransporterName, o => o.MapFrom(l => l.Transporter))));

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

        public async Task<IBusinessLogicResult<EditOfferViewModel>> GetOfferForEditAsync(int offerId , int getterUserId)
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

                Offer offer;

                // Critical Database
                try
                {
                    offer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(o => o.Id == offerId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (offer == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                Transporter transporter;
                try
                {
                    transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.Id == offer.TransporterId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (transporter.UserId != getterUserId)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied,
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

        public async Task<IBusinessLogicResult<EditOfferViewModel>> DeleteOfferAsync(int offerId, int deleterUserId)
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

                Offer offer;
                // Critical Database
                try
                {
                    offer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(o => o.Id == offerId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // User Verification
                if (offer == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                Transporter transporter;
                try
                {
                    transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.Id == offer.TransporterId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                if (transporter.UserId != deleterUserId)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.AccessDenied,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                try
                {
                    await _offerRepository.DeleteAsync(offer, true);
                }
                catch (Exception exception)
                {

                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                var editOfferViewModel = await _utility.MapAsync<Offer, EditOfferViewModel>(offer);
                messages.Add(new BusinessLogicMessage(type: MessageType.Info, MessageId.EntitySuccessfullyDeleted));
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

        public async Task<IBusinessLogicResult<EditOfferViewModel>> EditOfferAsync(EditOfferViewModel editOfferViewModel, int editorUserId)
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

                Offer offer;

                try
                {
                    try
                    {
                        offer = await _offerRepository.DeferredSelectAll().SingleOrDefaultAsync(o => o.Id == editOfferViewModel.Id);
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

                    Transporter transporter;
                    try
                    {
                        transporter = await _transporterRepository.DeferredSelectAll().SingleOrDefaultAsync(t => t.Id == offer.TransporterId);
                    }
                    catch (Exception exception)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                        return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                            messages: messages, exception: exception);
                    }

                    if (transporter.UserId != editorUserId)
                    {
                        messages.Add(new BusinessLogicMessage(type: MessageType.Error,
                                message: MessageId.AccessDenied, BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                           messages: messages);
                    }

                    editOfferViewModel.TransporterId = transporter.Id;
                    editOfferViewModel.ProjectId = offer.ProjectId;

                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);

                }

                //offer.Price = editOfferViewModel.Price;
                //offer.EstimatedTime = editOfferViewModel.EstimatedTime;
                //offer.Description = editOfferViewModel.Description;


                try
                {
                    await _utility.MapAsync(editOfferViewModel, offer);

                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                try
                {
                    await _offerRepository.UpdateAsync(offer, true); //, propertiesToBeUpdate.ToArray()
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<EditOfferViewModel>(succeeded: false, result: editOfferViewModel,
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

        public async Task<IBusinessLogicResult<OfferDetailsViewModel>> GetOfferDetailsAsync(int offerId)
        {
            var messages = new List<IBusinessLogicMessage>();
            try
            {
                Offer  offer;
                // Critical Database
                try
                {
                    offer = await _offerRepository.FindAsync(offerId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<OfferDetailsViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                // Verification
                if (offer == null)
                {
                    messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.ProjectNotFound,
                        BusinessLogicSetting.UserDisplayName));
                    return new BusinessLogicResult<OfferDetailsViewModel>(succeeded: false, result: null,
                        messages: messages);
                }

                var offerDetailsViewModel = await _utility.MapAsync<Offer, OfferDetailsViewModel>(offer);
                try
                {
                    var transporter = await _transporterRepository.FindAsync(offer.TransporterId);
                    var user = await _userRepository.FindAsync(transporter.UserId);
                    offerDetailsViewModel.TransporterName = user.Name;
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<OfferDetailsViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                return new BusinessLogicResult<OfferDetailsViewModel>(succeeded: true, result: offerDetailsViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                return new BusinessLogicResult<OfferDetailsViewModel>(succeeded: false, result: null,
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
