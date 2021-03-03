using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Advert.Api.Services;
using Advert.Models;
using Microsoft.AspNetCore.Mvc;

namespace Advert.Api.Controllers
{
  [ApiController]
  [Route("adverts/v1")]
  public class AdvertController : ControllerBase
  {
    private readonly IAdvertStorageService _advertStorageService;

    public AdvertController(IAdvertStorageService advertStorageService)
    {
      _advertStorageService = advertStorageService;
    }

    [HttpPost]
    [Route("Create")]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(CreateAdvertResponse), 200)]
    public async Task<IActionResult> Create(AdvertModel model)
    {
      string recordId;
      try
      {
        recordId = await _advertStorageService.Add(model);
      }
      catch (KeyNotFoundException)
      {
        return new NotFoundResult();
      }
      catch (Exception exception)
      {
        return StatusCode(500, exception.Message);
        throw;
      }

      return StatusCode(2001, new CreateAdvertResponse { Id = recordId });
    }

    [HttpPut]
    [Route("Confirm")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
    {
      try
      {
        await _advertStorageService.Confirm(model);
      }
      catch (KeyNotFoundException)
      {
        return new NotFoundResult();
      }
      catch (Exception exception)
      {
        return StatusCode(500, exception.Message);
        throw;
      }

      return new OkResult();
    }
  }
}