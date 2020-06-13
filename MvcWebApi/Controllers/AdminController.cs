using System.Threading.Tasks;
using BusinessLogic.Abstractions;
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

        public AdminController(IBusinessLogicUserManager businessLogicUserManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
        }

        [HttpGet]
        // [Authorize/*(Policy = "Admin, DeveloperSupport")*/]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UsersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.GetUsersAsync(1, page, pageSize, search, sort, filter);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.DeleteUserAsync(userId, HttpContext.GetCurrentUserId());
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        // [Authorize/*(Policy = "Admin, DeveloperSupport")*/]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeactivateUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.DeactivateUserAsync(userId, HttpContext.GetCurrentUserId());
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        // [Authorize/*(Policy = "Admin, DeveloperSupport")*/]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ActivateUser(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.ActivateUserAsync(userId, HttpContext.GetCurrentUserId());
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }
    }
}