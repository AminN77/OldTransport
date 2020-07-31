using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Microsoft.AspNetCore.Mvc;
using ViewModels;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IBusinessLogicProjectManager _businessLogicProjectManager;
        private readonly IBusinessLogicFeedbackManager _businessLogicFeedbackManager;
        private readonly IBusinessLogicSettingsManager _businessLogicSettingsManager;

        public HomeController(IBusinessLogicProjectManager businessLogicProjectManager,
            IBusinessLogicFeedbackManager businessLogicFeedbackManager, IBusinessLogicSettingsManager businessLogicSettingsManager)
        {
            _businessLogicProjectManager = businessLogicProjectManager;
            _businessLogicFeedbackManager = businessLogicFeedbackManager;
            _businessLogicSettingsManager = businessLogicSettingsManager;
        }

        [HttpGet]
        public async Task<IActionResult> IndexSettings()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicSettingsManager.GetIndexSettings();
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> TermsAndConditions()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicSettingsManager.GetTermsAndConditions();
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> HowItWorksList()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicSettingsManager.ListHowItWorksAsync();
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

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ContactUs(ContactUsViewModel contactUsViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicFeedbackManager.ContactUsAsync(contactUsViewModel);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Countries()
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicProjectManager.GetCountriesAsync();
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> Citiies(int countryId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicProjectManager.GetCitiesAsync(countryId);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }
    }
}
