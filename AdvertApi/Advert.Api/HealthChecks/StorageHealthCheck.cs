using System.Threading;
using System.Threading.Tasks;
using Advert.Api.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Advert.Api.HealthChecks
{
  public class StorageHealthCheck : IHealthCheck
  {
    private readonly IAdvertStorageService _storageService;

    public StorageHealthCheck(IAdvertStorageService storageService)
    {
      _storageService = storageService;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var result = await _storageService.CheckHealthAsync();
        return result ? HealthCheckResult.Healthy(): HealthCheckResult.Unhealthy();
    }
  }
}