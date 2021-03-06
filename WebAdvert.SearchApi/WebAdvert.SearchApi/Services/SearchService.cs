using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Elasticsearch.Net;
using Nest;
using WebAdvert.SearchApi.Models;
using WebAdvert.SearchApi.Services.Interfaces;

namespace WebAdvert.SearchApi.Services
{
  public class SearchService : ISearchService
  {
    private readonly IElasticClient _client;
    public SearchService(IElasticClient client)
    {
      _client = client;
    }

    public async Task<bool> CheckHealthAsync()
    {
      var result = await _client.Cluster.HealthAsync();
      return result.Status == Health.Green ? true : false;
    }

    public async Task<List<AdvertType>> Search(string keyword)
    {
      var searchResponse = await _client.SearchAsync<AdvertType>(search => search.
                                            Query(query => query.
                                                Term(field => field.Title, keyword.ToLower())
                                            ));

      return searchResponse.Hits.Select(hit => hit.Source).ToList();
    }
  }
}