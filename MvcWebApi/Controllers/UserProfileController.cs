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
    public class UserProfileController : ControllerBase
    {
        private readonly IBusinessLogicUserManager _businessLogicUserManager;

        public UserProfileController(IBusinessLogicUserManager businessLogicUserManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UserDetails(int userId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicUserManager.GetUserDetailsAsync(userId, getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }
    }
}