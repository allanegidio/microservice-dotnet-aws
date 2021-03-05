using System;
using System.IO;
using System.Threading.Tasks;
using Advert.Models;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Clients.Interfaces;
using WebAdvert.Web.Clients.Requests;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.Services.Interfaces;

namespace WebAdvert.Web.Controllers
{
  public class AdvertManagementController : Controller
  {
    private readonly IFileUploader _fileUploader;
    private readonly IAdvertApiClient _client;
    private readonly IMapper _mapper;

    public AdvertManagementController(IFileUploader fileUploader, IAdvertApiClient client, IMapper mapper)
    {
      _mapper = mapper;
      _fileUploader = fileUploader;
      _client = client;
    }

    public async Task<IActionResult> Create(CreateAdvertViewModel model)
    {
      return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
    {
      if (ModelState.IsValid)
      {
        var request = _mapper.Map<CreateAdvertRequest>(model);
        var apiCallResponse = await _client.CreateAsync(request);
        var id = apiCallResponse.Id;
        string filePath = string.Empty;

        if (imageFile != null)
        {
          var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
          filePath = $"{id}/{fileName}";

          try
          {
            using (var readStream = imageFile.OpenReadStream())
            {
              var result = await _fileUploader.UploadFileAsync(filePath, readStream);

              if (!result)
                throw new Exception("Could not upload the image to file repository. Please see the logs for details.");
            }

            var confirmAdvertRequest = new ConfirmAdvertRequest() { Id = id, Status = AdvertStatus.Active };
            var isConfirmed = await _client.ConfirmAsync(confirmAdvertRequest);

            if(!isConfirmed)
                throw new Exception($"Cannot Confirm Advert of Id: {id}");

            return RedirectToAction("Index", "Home");
          }
          catch (Exception e)
          {
            var confirmAdvertRequest = new ConfirmAdvertRequest() { Id = id, Status = AdvertStatus.Peding };
            var isConfirmed = await _client.ConfirmAsync(confirmAdvertRequest);
            Console.WriteLine(e);
          }
        }
      }

      return View(model);
    }
  }
}