using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Cross.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MvcWebApi.Provider;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors("CorsPolicy")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IBusinessLogicUserManager _businessLogicUserManager;
        private readonly IBusinessLogicProjectManager _businessLogicProjectManager;

        public AdminController(IBusinessLogicUserManager businessLogicUserManager, IBusinessLogicProjectManager businessLogicProjectManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
            _businessLogicProjectManager = businessLogicProjectManager;
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.DeveloperSupport)]
        public async Task<IActionResult> UsersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetUsersAsync(getterUserId, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.DeveloperSupport)]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deactivatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.DeactivateUserAsync(userId, deactivatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.DeveloperSupport)]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var activatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.ActivateUserAsync(userId, activatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.DeveloperSupport)]
        public async Task<IActionResult> DeactivateProject(int projectId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deactivatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicProjectManager.DeactivateProjectAsync(projectId, deactivatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Policy = CustomRoles.DeveloperSupport)]
        public async Task<IActionResult> ActivateProject(int projectId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var activatorUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicProjectManager.ActivateProjectAsync(projectId, activatorUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }
    }
}