using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Advert.Api.Services;
using Advert.Models;
using Advert.Models.Messages;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Advert.Api.Controllers
{
  [ApiController]
  [Route("adverts/v1")]
  public class AdvertController : ControllerBase
  {
    private readonly IAdvertStorageService _advertStorageService;
    private readonly IConfiguration _configuration;

    public AdvertController(IAdvertStorageService advertStorageService, IConfiguration configuration)
    {
      _configuration = configuration;
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

      return StatusCode(201, new CreateAdvertResponse { Id = recordId });
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
        await RaiseAdvertConfirmedMessage(model);
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

    [HttpGet]
    [Route("GetAll")]
    [ProducesResponseType(200)]
    [EnableCors("AllOrigin")]
    public async Task<IActionResult> GetAll()
    {
      return new JsonResult(await _advertStorageService.GetAllAsync());
    }

    private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
    {
      var topicArn = _configuration.GetValue<string>("TopicArn");
      var dbModel = await _advertStorageService.GetByIdAsync(model.Id);

      using (var client = new AmazonSimpleNotificationServiceClient())
      {
        var message = new AdvertConfirmedMessage { Id = model.Id, Title = dbModel.Title };
        var messageJson = JsonSerializer.Serialize(message);
        await client.PublishAsync(topicArn, messageJson);
      }
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(404)]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetById(string id)
    {
      try
      {
        var advert = await _advertStorageService.GetByIdAsync(id);
        return new JsonResult(advert);
      }
      catch (KeyNotFoundException)
      {
        return new NotFoundResult();
      }
      catch (Exception)
      {
        return new StatusCodeResult(500);
      }
    }
  }
}