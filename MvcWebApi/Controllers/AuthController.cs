using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MvcWebApi.Provider;
using MvcWebApi.Providers;
using ViewModels;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IAntiForgeryCookieService _antiForgery;
        private readonly ITokenFactoryService _tokenFactoryService;
        private readonly IBusinessLogicUserManager _businessLogicUserManager;

        public AuthController(IUserAuthenticator userAuthenticator, ITokenStoreService tokenStoreService,
            ITokenFactoryService tokenFactoryService, IAntiForgeryCookieService antiForgery, IBusinessLogicUserManager businessLogicUserManager)
        {
            _userAuthenticator = userAuthenticator;
            _tokenStoreService = tokenStoreService;
            _antiForgery = antiForgery;
            _tokenFactoryService = tokenFactoryService;
            _businessLogicUserManager = businessLogicUserManager;
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EmailAuthentication(EmailViewModel emailViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res1 = await _userAuthenticator.DoesEmailExistAsync(emailViewModel);
            if (res1.Succeeded) return Ok(res1);
            if (!res1.Succeeded && res1.Exception.Message == "DeactivatedUser") return NotFound(res1);
            var res2 = await _businessLogicUserManager.AddUserAsync(emailViewModel);
            if (!res2.Succeeded) return StatusCode(500, res2);
            return StatusCode(201, res2);
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Login(SignInInfoViewModel signInInfoViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _userAuthenticator.IsUserAuthenticateAsync(signInInfoViewModel);
            if (!res.Succeeded) return NotFound(res);
            var result = await _tokenFactoryService.CreateJwtTokensAsync(res.Result);
            await _tokenStoreService.AddUserTokenAsync(res.Result, result.RefreshTokenSerial, result.AccessToken, null);
            _antiForgery.RegenerateAntiForgeryCookies(result.Claims);
            Response.Headers.Add("AccessToken", result.AccessToken);
            Response.Headers.Add("RefreshToken", result.RefreshToken);
            return Ok(res);
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EmailVerification(ActivationCodeViewModel activationCodeViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.VerifyActivationCodeAysnc(activationCodeViewModel);
            if (!res.Succeeded) return StatusCode(500, res);

            return Ok(res);
        }

        [HttpPut]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel userRegisterViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.UpdateUserRegisterInfoAsync(userRegisterViewModel);
            if (!res.Succeeded) return StatusCode(500, res);
            var result = await _tokenFactoryService.CreateJwtTokensAsync(res.Result);
            await _tokenStoreService.AddUserTokenAsync(res.Result, result.RefreshTokenSerial, result.AccessToken, null);
            _antiForgery.RegenerateAntiForgeryCookies(result.Claims);
            Response.Headers.Add("AccessToken", result.AccessToken);
            Response.Headers.Add("RefreshToken", result.RefreshToken);
            return Ok(res);
        }


        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ForgetPassword(UserForgetPasswordViewModel userForgetPasswordViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.ForgetPasswordAsync(userForgetPasswordViewModel);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> ResendEmail(EmailViewModel emailViewModel)
        {
            var res = await _businessLogicUserManager.ResendSendVerificationEmailAsync(emailViewModel);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }
    }
}