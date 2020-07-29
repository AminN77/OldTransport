using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MvcWebApi.Hubs;
using MvcWebApi.Hubs.Abstractions;
using MvcWebApi.Provider;
using ViewModels;

namespace MvcWebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("CorsPolicy")]
    public class OfferController : ControllerBase
    {
        private readonly IBusinessLogicOfferManager _businessLogicOfferManager;
        private readonly IPanelHub _panelHub;

        public OfferController(IBusinessLogicOfferManager businessLogicOfferManager,IPanelHub panelHub)
        {
            _businessLogicOfferManager = businessLogicOfferManager;
            _panelHub = panelHub;
        }

        [HttpPost]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddOffer(AddOfferViewModel addOfferViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var adderUserId = HttpContext.GetCurrentUserId();
            var result = await _businessLogicOfferManager.AddOfferAsync(addOfferViewModel, adderUserId);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OffersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicOfferManager.GetOffersAsync(page, pageSize, search, sort, filter);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> TransporterOffersList(int page, int pageSize, string search, string sort, string filter)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var result = await _businessLogicOfferManager.GetTransporterOffersAsync(page, pageSize, search, sort, filter, getterUserId);
            if (!result.Succeeded) return StatusCode(500, result);
            await _panelHub.UpdateOffersListRealTime();
            return Ok(result);
        }

        [HttpPut]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> EditOffer(EditOfferViewModel editOfferViewModel)
        {
            if (!ModelState.IsValid) return BadRequest();
            var editorUserId = HttpContext.GetCurrentUserId();
            var result = await _businessLogicOfferManager.EditOfferAsync(editOfferViewModel, editorUserId);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);

        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditOffer(int offerId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var getterUserId = HttpContext.GetCurrentUserId();
            var res = await _businessLogicOfferManager.GetOfferForEditAsync(offerId, getterUserId);
            if (!res.Succeeded) return StatusCode(500, res);
            return Ok(res);
        }

        [HttpDelete]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteOffer(int offerId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var deleterUserId = HttpContext.GetCurrentUserId();
            var result = await _businessLogicOfferManager.DeleteOfferAsync(offerId, deleterUserId);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }

        [HttpGet]
        [Authorize]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> OfferDetails(int offerId)
        {
            if (!ModelState.IsValid) return BadRequest();
            var result = await _businessLogicOfferManager.GetOfferDetailsAsync(offerId);
            if (!result.Succeeded) return StatusCode(500, result);
            return Ok(result);
        }
    }
}