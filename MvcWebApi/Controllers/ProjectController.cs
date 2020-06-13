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

            var res = await _businessLogicProjectManager.AddProjectAsync(addProjectViewModel, HttpContext.GetCurrentUserId());

            if (!res.Succeeded) return StatusCode(500, res);

            return Ok();
        }


        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ProjectsList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();

            var res = await _businessLogicProjectManager.GetProjectsAsync(1, page, pageSize, search, sort, filter);

            if (!res.Succeeded) return StatusCode(500, res);

            return Ok(res);

        }


        [HttpPut]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditProject(EditProjectViewModel editProjectViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var res = await _businessLogicProjectManager.EditProjectAsync(editProjectViewModel, HttpContext.GetCurrentUserId());

            if (!res.Succeeded) return StatusCode(500, res);

            return Ok();

        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditProject(int Id)
        {
            if (!ModelState.IsValid) return BadRequest();

            var res = await _businessLogicProjectManager.GetProjectForEditAsync(Id);

            if (!res.Succeeded) return StatusCode(500, res);

            return Ok(res);

        }


        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteProject(DeleteProjectViewModel deleteProjectViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var res = await _businessLogicProjectManager.DeleteProjectAsync(deleteProjectViewModel);

            if (!res.Succeeded) return StatusCode(500, res);

            return Ok("deleted");

        }


    }
}