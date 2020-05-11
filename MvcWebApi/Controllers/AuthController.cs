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