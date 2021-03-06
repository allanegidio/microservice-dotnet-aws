using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebAdvert.SearchApi.Services.Interfaces;

namespace WebAdvert.SearchApi.HealthChecks
{
  public class SearchHealthCheck : IHealthCheck
  {
    private readonly ISearchService _service;
    public SearchHealthCheck(ISearchService service)
    {
      _service = service;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
      var result = await _service.CheckHealthAsync();
      return result ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
    }
  }
}