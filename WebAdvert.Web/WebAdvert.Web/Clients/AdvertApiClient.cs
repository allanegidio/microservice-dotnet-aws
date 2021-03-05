using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Advert.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using WebAdvert.Web.Clients.Interfaces;
using WebAdvert.Web.Clients.Requests;

namespace WebAdvert.Web.Clients
{
  public class AdvertApiClient : IAdvertApiClient
  {
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly IMapper _mapper;

    public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
    {
      _configuration = configuration;
      _client = client;
      _mapper = mapper;
      var baseUrl = _configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");

      _client.BaseAddress = new Uri(baseUrl);
      _client.DefaultRequestHeaders.Add("Content-type", "application/json");
    }

    public async Task<WebAdvert.Web.Clients.Responses.CreateAdvertResponse> CreateAsync(CreateAdvertRequest request)
    {
      var advertApiModel = _mapper.Map<AdvertModel>(request);
      var jsonModel = JsonSerializer.Serialize<AdvertModel>(advertApiModel);
      var response = await _client.PostAsync(new Uri($"{_client.BaseAddress}/create"), new StringContent(jsonModel));
      var responseJson = await response.Content.ReadAsStringAsync();
      var createAdvertResponse = JsonSerializer.Deserialize<CreateAdvertResponse>(responseJson);
      var advertResponse = _mapper.Map<WebAdvert.Web.Clients.Responses.CreateAdvertResponse>(createAdvertResponse);

      return advertResponse;
    }

    public async Task<bool> ConfirmAsync(ConfirmAdvertRequest request)
    {
      var advertModel = _mapper.Map<ConfirmAdvertModel>(request);
      var jsonModel = JsonSerializer.Serialize(advertModel);
      var response = await _client.PutAsync(new Uri($"{_client.BaseAddress}/confirm"), new StringContent(jsonModel));
      var responseJson = await response.Content.ReadAsStringAsync();

      return response.StatusCode == HttpStatusCode.OK;
    }


  }
}