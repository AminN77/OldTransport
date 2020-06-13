using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Cross.Abstractions.EntityEnums;
using Data.Model;
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
        public async Task<IActionResult> UserDetails(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.GetUserDetailsAsync(userId, HttpContext.GetCurrentUserId());
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
    }
}