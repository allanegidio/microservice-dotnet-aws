using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Advert.Models;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using WebAdvert.Web.Clients.Interfaces;
using WebAdvert.Web.Clients.Requests;
using WebAdvert.Web.Models.AdvertManagement;

namespace WebAdvert.Web.Clients
{
  public class AdvertApiClient : IAdvertApiClient
  {
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;
    private readonly IMapper _mapper;
    private readonly string _baseAddress;

    public AdvertApiClient(IConfiguration configuration, HttpClient client, IMapper mapper)
    {
      _configuration = configuration;
      _client = client;
      _mapper = mapper;
      _baseAddress = _configuration.GetSection("AdvertApi").GetValue<string>("BaseUrl");
    }

    public async Task<WebAdvert.Web.Clients.Responses.CreateAdvertResponse> CreateAsync(CreateAdvertRequest request)
    {
      var advertApiModel = _mapper.Map<AdvertModel>(request);
      var jsonModel = JsonSerializer.Serialize<AdvertModel>(advertApiModel);
      var response = await _client.PostAsync(new Uri($"{_baseAddress}/Create"), new StringContent(jsonModel));
      var responseJson = await response.Content.ReadAsStringAsync();
      var createAdvertResponse = JsonSerializer.Deserialize<CreateAdvertResponse>(responseJson);
      var advertResponse = _mapper.Map<WebAdvert.Web.Clients.Responses.CreateAdvertResponse>(createAdvertResponse);

      return advertResponse;
    }

    public async Task<bool> ConfirmAsync(ConfirmAdvertRequest request)
    {
      var advertModel = _mapper.Map<ConfirmAdvertModel>(request);
      var jsonModel = JsonSerializer.Serialize(advertModel);
      var response = await _client.PutAsync(new Uri($"{_baseAddress}/Confirm"), new StringContent(jsonModel));
      var responseJson = await response.Content.ReadAsStringAsync();

      return response.StatusCode == HttpStatusCode.OK;
    }

    public async Task<List<Advertisement>> GetAllAsync()
    {
      var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/GetAll"));
      var allAdvertModels = await apiCallResponse.Content.ReadAsAsync<List<AdvertModel>>();

      return allAdvertModels.Select(x => _mapper.Map<Advertisement>(x)).ToList();
    }

    public async Task<Advertisement> GetByIdAsync(string advertId)
    {
      var apiCallResponse = await _client.GetAsync(new Uri($"{_baseAddress}/{advertId}"));
      var advertModel = await apiCallResponse.Content.ReadAsAsync<AdvertModel>();

      return _mapper.Map<Advertisement>(advertModel);
    }
  }
}