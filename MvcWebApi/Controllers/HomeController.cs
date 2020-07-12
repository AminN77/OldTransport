using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IBusinessLogicUserManager _businessLogicUserManager;
        private readonly IBusinessLogicProjectManager _businessLogicProjectManager;
        private readonly IBusinessLoginFeedbackManager _businessLogicFeedbackManager;

        public HomeController(IBusinessLogicUserManager businessLogicUserManager, IBusinessLogicProjectManager businessLogicProjectManager,
            IBusinessLoginFeedbackManager businessLogicFeedbackManager)
        {
            _businessLogicUserManager = businessLogicUserManager;
            _businessLogicProjectManager = businessLogicProjectManager;
            _businessLogicFeedbackManager = businessLogicFeedbackManager;
        }

        [HttpGet]
        public async Task<IActionResult> IndexSettings()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.AdminGetIndexSettings();
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> HowItWorks()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.AdminGetHowItWorks();
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> TermsAndConditions()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicUserManager.AdminGetTermsAndConditions();
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectsCount()
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicProjectManager.CountProjectsAsync();
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }
    }
}
