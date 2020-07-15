using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Cross.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MvcWebApi.Provider;
using ViewModels;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBusinessLogicUserManager _businessLogicUserManager;
        private readonly IBusinessLogicProjectManager _businessLogicProjectManager;
        private readonly IBusinessLoginFeedbackManager _businessLogicFeedbackManager;
        private readonly IBusinessLogicSettingsManager _businessLogicSettingsManager;

        public AdminController(IBusinessLogicUserManager businessLogicUserManager, IBusinessLogicProjectManager businessLogicProjectManager,
            IBusinessLoginFeedbackManager businessLogicFeedbackManager, IBusinessLogicSettingsManager businessLogicSettingsManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
            _businessLogicProjectManager = businessLogicProjectManager;
            _businessLogicFeedbackManager = businessLogicFeedbackManager;
            _businessLogicSettingsManager = businessLogicSettingsManager;
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> UsersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetUsersAsync(getterUserId, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deactivatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.DeactivateUserAsync(userId, deactivatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var activatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.ActivateUserAsync(userId, activatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> DeactivateProject(int projectId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deactivatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicProjectManager.DeactivateProjectAsync(projectId, deactivatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> ActivateProject(int projectId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var activatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicProjectManager.ActivateProjectAsync(projectId, activatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> Settings()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicSettingsManager.AdminGetSettingsForEdit();
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> Settings(SettingsViewModel settingsViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicSettingsManager.AdminEditSettings(settingsViewModel);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deleterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.DeleteUserAsync(userId, deleterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> MerchantsList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetMerchantsAsync(getterUserId, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> TransportersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetTransportersAsync(getterUserId, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> GetFeedback(int feedbackId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicFeedbackManager.GetFeedbackDetailsAsync(feedbackId, getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> FeedbacksList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicFeedbackManager.GetFeedbacksAsync(getterUserId, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> ContactMessagesList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicFeedbackManager.GetContactMessagesAsync(getterUserId, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.Admin)]
        public async Task<IActionResult> GetContactMessage(int messageId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicFeedbackManager.GetContactMessageAsync(messageId, getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }
    }
}