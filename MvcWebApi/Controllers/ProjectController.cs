using System;
using System.Collections.Generic;
using System.Linq;
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
    public class ProjectController : ControllerBase
    {
        private readonly IBusinessLogicProjectManager _businessLogicProjectManager;

        public ProjectController(IBusinessLogicProjectManager businessLogicProjectManager)
        {
            _businessLogicProjectManager = businessLogicProjectManager;
        }



        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddProject(AddProjectViewModel addProjectViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicProjectManager.AddProjectAsync(addProjectViewModel, HttpContext.GetCurrentUserId());

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);
        }


        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ProjectsList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicProjectManager.GetProjectsAsync(1, page, pageSize, search, sort, filter);

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);

        }


        [HttpPut]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditProject(EditProjectViewModel editProjectViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicProjectManager.EditProjectAsync(editProjectViewModel, HttpContext.GetCurrentUserId());

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);

        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditProject(int Id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicProjectManager.GetProjectForEditAsync(Id,HttpContext.GetCurrentUserId());

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);

        }


        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteProject(int projectId)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicProjectManager.DeleteProjectAsync(projectId,HttpContext.GetCurrentUserId());

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);

        }


    }
}