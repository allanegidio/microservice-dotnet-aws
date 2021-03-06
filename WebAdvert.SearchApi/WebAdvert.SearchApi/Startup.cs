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
using WebAdvert.SearchApi.Extensions;
using WebAdvert.SearchApi.HealthChecks;
using WebAdvert.SearchApi.Services;
using WebAdvert.SearchApi.Services.Interfaces;

namespace WebAdvert.SearchApi
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
            services.AddElasticSearch(Configuration);
            services.AddTransient<ISearchService, SearchService>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebAdvert.SearchApi", Version = "v1" });
            });
            services.AddHealthChecks().AddCheck<SearchHealthCheck>("Search");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAdvert.SearchApi v1"));
            }

            loggerFactory.AddAWSProvider(Configuration.GetAWSLoggingConfigSection(), formatter: (logLevel, loggerMessage, exception) => {
                return $"[{DateTime.Now} {logLevel} {loggerMessage} {exception?.Message} {exception?.StackTrace}";
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHealthChecks("/Health");
        }
    }
}
