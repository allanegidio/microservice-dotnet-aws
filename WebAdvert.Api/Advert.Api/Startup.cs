using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using AutoMapper;
using Advert.Api.Mapper;
using Advert.Api.Services;
using Advert.Api.HealthChecks;
using Amazon.Util;
using Amazon.ServiceDiscovery;
using Amazon.ServiceDiscovery.Model;

namespace Advert.Api
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddAutoMapper(typeof(AdvertProfile));

      services.AddTransient<IAdvertStorageService, AdvertDynamoDbStorage>();

      services.AddControllers();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Advert.Api", Version = "v1" });
      });

      services.AddHealthChecks().AddCheck<StorageHealthCheck>("Storage");
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public async Task Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Advert.Api v1"));
      }

      app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });

      app.UseHealthChecks("/Health");

      await RegisterToCloudMap();
    }

    private async Task RegisterToCloudMap()
    {
      const string serviceId = "Put your CloudMap service ID here.";
      var instanceId = EC2InstanceMetadata.InstanceId;

      if (!string.IsNullOrEmpty(instanceId))
      {
        var ipv4 = EC2InstanceMetadata.PrivateIpAddress;
        var client = new AmazonServiceDiscoveryClient();

        await client.RegisterInstanceAsync(new RegisterInstanceRequest
        {
          InstanceId = instanceId,
          ServiceId = serviceId,
          Attributes = new Dictionary<string, string>() 
          { 
            { "AWS_INSTANCE_IPV4", ipv4 },
            { "AWS_INSTANCE_PORT", "80"}
          }
        });
      }
    }
  }
}
