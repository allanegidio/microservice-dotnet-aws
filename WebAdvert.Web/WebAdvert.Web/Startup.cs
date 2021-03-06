using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;
using Polly.Extensions.Http;
using WebAdvert.Web.Clients;
using WebAdvert.Web.Clients.Interfaces;
using WebAdvert.Web.Mappers;
using WebAdvert.Web.Services;
using WebAdvert.Web.Services.Interfaces;

namespace WebAdvert.Web
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
      services.AddCognitoIdentity(config =>
      {
        config.Password = new PasswordOptions
        {
          RequiredLength = 6,
          RequiredUniqueChars = 0,
          RequireDigit = false,
          RequireLowercase = false,
          RequireNonAlphanumeric = false,
          RequireUppercase = false
        };
      });

      services.ConfigureApplicationCookie(option =>
      {
        option.LoginPath = "/Account/Login";
      });

      services.AddAutoMapper(typeof(AdvertApiProfile));

      // Dependency Injection
      services.AddTransient<IFileUploader, S3FileUploader>();


      // Circuit Breaker
      services.AddHttpClient<IAdvertApiClient, AdvertApiClient>()
          .AddPolicyHandler(GetRetryPolicy())
          .AddPolicyHandler(GetCircuitBreakerPatternPolicy());


      services.AddControllersWithViews();
    }

    private IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPatternPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
                                    .CircuitBreakerAsync(3, TimeSpan.FromSeconds(3));
    }

    private IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions.HandleTransientHttpError()
                                .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
                                .WaitAndRetryAsync(5, retryAttempy => TimeSpan.FromSeconds(Math.Pow(2, retryAttempy)));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseStaticFiles();
      app.UseRouting();
      app.UseAuthentication();
      app.UseAuthorization();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllerRoute(
                  name: "default",
                  pattern: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
