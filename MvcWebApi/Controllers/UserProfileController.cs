using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Cross.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebApi.Provider;
using ViewModels;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IBusinessLogicUserManager _businessLogicUserManager;

        public UserProfileController(IBusinessLogicUserManager businessLogicUserManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserDetails(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetUserDetailsAsync(userId, getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deleterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.DeleteUserAsync(userId, deleterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ChangePassword(UserChangePasswordViewModel userChangePasswordViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res1 = await _businessLogicUserManager.ChangePasswordAsync(userChangePasswordViewModel, userId);
            if (!res1.Succeeded) return StatusCode(500, res1);
            return Ok();
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel editUserViewModel, IFormFile file)
        {
            if (!ModelState.IsValid) return BadRequest();
            var editorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.EditUserAsync(editUserViewModel, editorUserId, file);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> IsUserMerchant()
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.MerchantAuthenticator(userId);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> IsUserTransporter()
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.TransporterAuthenticator(userId);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddTransporter(AddTransporterViewModel addTransporterViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.AddTransporterAsync(addTransporterViewModel);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddMerchant(AddMerchantViewModel addMerchantViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.AddMerchantAsync(addMerchantViewModel);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }
    }
}