using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MvcWebApi.Providers;
using ViewModels;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAuthenticator _userAuthenticator;
        private readonly ITokenStoreService _tokenStoreService;
        private readonly IAntiForgeryCookieService _antiForgery;
        private readonly ITokenFactoryService _tokenFactoryService;

        public AuthController(IUserAuthenticator userAuthenticator, ITokenStoreService tokenStoreService,
            ITokenFactoryService tokenFactoryService, IAntiForgeryCookieService antiForgery)
        {
            _userAuthenticator = userAuthenticator;
            _tokenStoreService = tokenStoreService;
            _antiForgery = antiForgery;
            _tokenFactoryService = tokenFactoryService;
        }

        [HttpPost]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EmailAuthentication(EmailViewModel emailViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res1 = await _userAuthenticator.DoesEmailExistAsync(emailViewModel);
            if (res1.Succeeded) return Ok(new { res1 });

            var res2 = await _businessLogicUserManager.AddUserAsync(emailViewModel);
            if (!res2.Succeeded) return StatusCode(500, res2);

            return Ok(new { res2 });
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
            return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
        }
    }
}