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
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class OfferController : ControllerBase
    {
        private readonly IBusinessLogicOfferManager _businessLogicOfferManager;

        public OfferController(IBusinessLogicOfferManager businessLogicOfferManager)
        {
            _businessLogicOfferManager = businessLogicOfferManager;
        }



        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddOffer(AddOfferViewModel addOfferViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicOfferManager.AddOfferAsync(addOfferViewModel,HttpContext.GetCurrentUserId());

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OffersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();

            var result = await _businessLogicOfferManager.GetOfferAsync(1, page, pageSize, search, sort, filter);

            if (!result.Succeeded) return StatusCode(500, result);

            return Ok(result);

        }


    }
}