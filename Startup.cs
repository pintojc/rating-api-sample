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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.ApplicationInsights;
using RatingAPI.services.lookupService;
using RatingAPI.services.ratingService;
using RatingAPI.services.underwritingService;


namespace RatingAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            this.ConfigureLogger(configuration);
        }

        public IConfiguration Configuration { get; }

        public void ConfigureLogger (IConfiguration configuration)
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.WithExceptionDetails()
                .WriteTo.File("Logs/log-.txt", rollingInterval:RollingInterval.Day);

            var appInsightsKey = configuration["APPINSIGHTS_INSTRUMENTATIONKEY"];
            if(appInsightsKey != null)
            {
                loggerConfig.WriteTo.ApplicationInsightsEvents(appInsightsKey, restrictedToMinimumLevel:LogEventLevel.Information);
            }

            Log.Logger = loggerConfig.CreateLogger();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions( c => {
                    c.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    c.AllowInputFormatterExceptionMessages = true;
                });

            services.AddSwaggerGen( c => 
            {
                c.SwaggerDoc("v1", new Info{
                    Title = "Rating API",
                    Description = "Standalone Rating Engine API",
                    Version = "1.0.0"
                });
            });


            services.AddSingleton<ILookupServiceFactory, LookupProviderService>();
            services.AddSingleton<IRatingServiceFactory, RatingServiceFactory>();
            //services.AddSingleton<IUnderwritingServiceFactory>();

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            
            loggerFactory.AddSerilog();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseHttpsRedirection();

            app.UseSwagger();
            app.UseSwaggerUI( c => {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Rating Engine API v1");
                c.RoutePrefix = string.Empty;
            });
            app.UseMvc();
        }
    }
}
