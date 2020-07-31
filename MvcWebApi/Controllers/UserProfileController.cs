using System.Threading.Tasks;
using BusinessLogic.Abstractions;
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
        private readonly IBusinessLogicRoleManager _businessLogicRoleManager;
        private readonly IBusinessLogicFeedbackManager _businessLogicFeedbackManager;

        public UserProfileController(IBusinessLogicUserManager businessLogicUserManager, IBusinessLogicRoleManager businessLogicRoleManager,
            IBusinessLogicFeedbackManager businessLogicFeedbackManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
            _businessLogicRoleManager = businessLogicRoleManager;
            _businessLogicFeedbackManager = businessLogicFeedbackManager;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> UserDetails(int? userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetUserDetailsAsync(userId, getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteAccount(int? userId)
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

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditUser()
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetUserForEditAsync(getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpPut]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel editUserViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var editorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.EditUserAsync(editUserViewModel, editorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UploadPhoto(IFormFile formFile)
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicUserManager.UploadPhotoAsync(formFile);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> IsUserMerchant()
        {
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.MerchantAuthenticator(userId);
            if (!res.Succeeded) return StatusCode(204, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> IsUserTransporter()
        {
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.TransporterAuthenticator(userId);
            if (!res.Succeeded) return StatusCode(204, res);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddTransporter(AddTransporterViewModel addTransporterViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.AddTransporterAsync(userId, addTransporterViewModel);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddMerchant(AddMerchantViewModel addMerchantViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.AddMerchantAsync(userId, addMerchantViewModel);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CurrentlyAuthenticatedUsersRole()
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicRoleManager.FindUserRolesAsync(userId);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Feedback(SendFeedbackViewModel sendFeedbackViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var userId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicFeedbackManager.SendFeedback(userId, sendFeedbackViewModel);
            if (!res.Succeeded) return NotFound(res);
            return Ok(res);
        }
    }
}