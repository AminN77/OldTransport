using BusinessLogic.Abstractions;
using BusinessLogic.Abstractions.Message;
using Data.Abstractions;
using Data.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ViewModels;

namespace BusinessLogic
{
    public class BusinessLogicUserTokenManager : IBusinessLogicUserTokenManager
    {
        private readonly IRepository<UserToken> _userTokenRepository;
        private readonly BusinessLogicUtility _utility;
        private readonly ISecurityProvider _securityProvider;


        public BusinessLogicUserTokenManager(IRepository<UserToken> userTokenRepository, BusinessLogicUtility utility, ISecurityProvider securityProvider)
        {
            _userTokenRepository = userTokenRepository;
            _utility = utility;
            _securityProvider = securityProvider;
        }

        public async Task<IBusinessLogicResult<AddUserTokenViewModel>> AddUserTokenAsync(AddUserTokenViewModel addTokenViewModel, int adderUserId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Safe Map
                var userToken = await _utility.MapAsync<AddUserTokenViewModel, UserToken>(addTokenViewModel);
                // Safe initialization
                userToken.RefreshTokenIdHash = await _securityProvider.Sha256HashAsync(addTokenViewModel.RefreshTokenIdHash);

                userToken.RefreshTokenIdHashSource = string.IsNullOrWhiteSpace(addTokenViewModel.RefreshTokenIdHashSource) ?
                                           null : await _securityProvider.Sha256HashAsync(addTokenViewModel.RefreshTokenIdHashSource);
                userToken.AccessTokenHash = await _securityProvider.Sha256HashAsync(addTokenViewModel.AccessTokenHash);
                // Critical Database operation
                try
                {
                    await _userTokenRepository.AddAsync(userToken);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.UserSuccessfullyAdded));
                return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: true, result: addTokenViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }
        }


        public async Task<IBusinessLogicResult> DeleteExpiredTokensAsync()
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Database operation
                bool result;
                try
                {
                    var now = DateTimeOffset.UtcNow;
                    var expiredTokens = await _userTokenRepository.DeferredWhere(usrToken => usrToken.RefreshTokenExpiresDateTime < now).ToListAsync();
                    result = await _userTokenRepository.DeleteAllAsync(expiredTokens);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.UserSuccessfullyAdded));
                return new BusinessLogicResult(succeeded: result,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult(succeeded: false,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> DeleteTokenAsync(string refreshTokenId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Database operation
                UserToken token;
                try
                {
                    var refreshTokenIdHash = await _securityProvider.Sha256HashAsync(refreshTokenId);
                    token = await _userTokenRepository.FindAsync(refreshTokenIdHash);
                    if (token == null)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                bool result;
                try
                {
                    result = await _userTokenRepository.DeleteAsync(token);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.UserSuccessfullyAdded));
                return new BusinessLogicResult(succeeded: result,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult(succeeded: false,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> DeleteTokensWithSameRefreshTokenSourceAsync(string refreshTokenIdHashSource)
        {

            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Database operation
                IList<UserToken> listTokens;
                try
                {
                    listTokens = await _userTokenRepository.DeferredWhere(userToken =>
                        userToken.RefreshTokenIdHashSource == refreshTokenIdHashSource).ToListAsync();

                    if (listTokens == null)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                bool result;
                try
                {
                    result = await _userTokenRepository.DeleteAllAsync(listTokens);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<AddUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.UserSuccessfullyAdded));
                return new BusinessLogicResult(succeeded: result,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult(succeeded: false,
                    messages: messages, exception: exception);
            }
        }

       

        public async Task<IBusinessLogicResult<ListUserTokenViewModel>> FindTokenAsync(string refreshTokenId)
        {
              var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Database operation
                string refreshTokenIdHash;
                try
                {
                    refreshTokenIdHash = await _securityProvider.Sha256HashAsync(refreshTokenId);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<ListUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }
                UserToken result;
                try
                {
                    result = await _userTokenRepository.DeferredWhere(userToken =>
                        userToken.RefreshTokenIdHash == refreshTokenIdHash).FirstOrDefaultAsync();
                    //return _tokens.Include(x => x.User).FirstOrDefaultAsync(x => x.RefreshTokenIdHash == refreshTokenIdHash);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult<ListUserTokenViewModel>(succeeded: false, result: null,
                        messages: messages, exception: exception);
                }

                var userTokenViewModel = await _utility.MapAsync<UserToken, ListUserTokenViewModel>(result);

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.EntitySuccessfullyAdded));
                return new BusinessLogicResult<ListUserTokenViewModel>(succeeded: true,
                    result: userTokenViewModel,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult<ListUserTokenViewModel>(succeeded: false, result: null,
                    messages: messages, exception: exception);
            }

        }

        public async Task<IBusinessLogicResult> InvalidateUserTokensAsync(int userId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Database operation
                IList<UserToken> listTokens;
                try
                {
                    listTokens = await _userTokenRepository.DeferredWhere(userToken => userToken.UserId == userId).ToListAsync();
                    if (listTokens == null)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false,
                        messages: messages, exception: exception);
                }

                bool result;
                try
                {
                    result = await _userTokenRepository.DeleteAllAsync(listTokens);
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false,
                        messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.UserSuccessfullyAdded));
                return new BusinessLogicResult(succeeded: result,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult(succeeded: false,
                    messages: messages, exception: exception);
            }
        }

        public async Task<IBusinessLogicResult> IsValidTokenAsync(string accessToken, int userId)
        {
            var messages = new List<BusinessLogicMessage>();
            try
            {
                // Critical Database operation
                UserToken userToken;
                try
                {
                    var accessTokenHash = await _securityProvider.Sha256HashAsync(accessToken);

                    userToken = await _userTokenRepository
                        .DeferredWhere(usrToken => usrToken.AccessTokenHash == accessTokenHash && usrToken.UserId == userId)
                        .FirstOrDefaultAsync();

                    if (userToken == null)
                    {
                        messages.Add(new BusinessLogicMessage(MessageType.Error, MessageId.EntityDoesNotExist,
                            BusinessLogicSetting.UserDisplayName));
                        return new BusinessLogicResult(succeeded: false, messages: messages);
                    }
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false,
                        messages: messages, exception: exception);
                }

                bool result;
                try
                {
                    result = userToken.AccessTokenExpiresDateTime >= DateTimeOffset.UtcNow;
                }
                catch (Exception exception)
                {
                    messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.InternalError));
                    return new BusinessLogicResult(succeeded: false,
                        messages: messages, exception: exception);
                }

                messages.Add(new BusinessLogicMessage(type: MessageType.Info,
                    message: MessageId.UserSuccessfullyAdded));
                return new BusinessLogicResult(succeeded: result,
                    messages: messages);
            }
            catch (Exception exception)
            {
                messages.Add(new BusinessLogicMessage(type: MessageType.Error, message: MessageId.Exception));
                return new BusinessLogicResult(succeeded: false,
                    messages: messages, exception: exception);
            }
        }

        public void Dispose()
        {
            _userTokenRepository.Dispose();
        }
    }
}
