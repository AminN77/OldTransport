using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Cross.Abstractions;
using Microsoft.AspNetCore.Http;
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
        private readonly IFileService _fileService;

        public HomeController(IBusinessLogicProjectManager businessLogicProjectManager, IFileService fileService,
            IBusinessLogicFeedbackManager businessLogicFeedbackManager, IBusinessLogicSettingsManager businessLogicSettingsManager)
        {
            _businessLogicProjectManager = businessLogicProjectManager;
            _businessLogicFeedbackManager = businessLogicFeedbackManager;
            _businessLogicSettingsManager = businessLogicSettingsManager;
            _fileService = fileService;
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
        public async Task<IActionResult> HowItWorks()
        {
            if (!ModelState.IsValid) return BadRequest();
            var res = await _businessLogicSettingsManager.GetHowItWorks();
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

        //[HttpPost]
        //[IgnoreAntiforgeryToken]
        //public IActionResult Test(IFormFile formFile)
        //{
        //    if (!ModelState.IsValid) return BadRequest();
        //    var result = _fileService.SaveFile(formFile, Cross.Abstractions.EntityEnums.FileTypes.ProfilePhoto);
        //    //if (!result.Succeeded) return StatusCode(500, result);
        //    return Ok(result);
        //}
    }
}
