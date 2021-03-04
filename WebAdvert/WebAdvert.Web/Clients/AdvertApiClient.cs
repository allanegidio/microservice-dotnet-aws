using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Advert.Models;
using Microsoft.Extensions.Configuration;
using WebAdvert.Web.Clients.Interfaces;
using WebAdvert.Web.Clients.Requests;

namespace WebAdvert.Web.Clients
{
  public class AdvertApiClient : IAdvertApiClient
  {
    private readonly IConfiguration _configuration;
    private readonly HttpClient _client;

    public AdvertApiClient(IConfiguration configuration, HttpClient client)
    {
      _configuration = configuration;
      _client = client;

      var createUrl = _configuration.GetSection("AdvertApi").GetValue<string>("CreateUrl");

      _client.BaseAddress = new Uri(createUrl);
      _client.DefaultRequestHeaders.Add("Content-type", "application/json");
    }
    public async Task<CreateAdvertResponse> Create(CreateAdvertRequest model)
    {
        var advertApiRequest = new CreateAdvertRequest();
        var jsonModel = JsonSerializer.Serialize<CreateAdvertRequest>(advertApiRequest);
        var response = await _client.PostAsync(_client.BaseAddress, new StringContent(jsonModel));
        var responseJson = await response.Content.ReadAsStringAsync();
        var createAdvertResponse = JsonSerializer.Deserialize<CreateAdvertResponse>(responseJson);
        var advertResponse = new CreateAdvertResponse();

        return advertResponse;
    }
  }
}