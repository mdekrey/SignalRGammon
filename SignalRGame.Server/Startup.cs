using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SignalRGame.GameUtilities;
using Jaeger;
using Jaeger.Samplers;
using OpenTracing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SignalRGame
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOpenTracingCoreServices(builder =>
            {
                builder.AddAspNetCore()
                    .AddCoreFx();
            });

            services.AddSingleton<ITracer>(serviceProvider =>
            {
                string serviceName = serviceProvider.GetRequiredService<IWebHostEnvironment>().ApplicationName;

                // This will log to a default localhost installation of Jaeger.
                var tracer = new Tracer.Builder(serviceName)
                    .WithSampler(new RemoteControlledSampler.Builder(serviceName).WithInitialSampler(new ConstSampler(true)).Build())
                    .Build();

                // Allows code that can't use DI to also access the tracer.
                OpenTracing.Util.GlobalTracer.Register(tracer);

                return tracer;
            });

            services.AddSingleton<ILoggerProvider, Logging.OpenTracingLoggerProvider>();

            services.AddMemoryCache();
            var signalr = services.AddSignalR();
            if (!string.IsNullOrEmpty(configuration["Azure:SignalR:ConnectionString"]))
                signalr.AddAzureSignalR(configuration["Azure:SignalR:ConnectionString"]);
            services.AddGameFactory();
            services.AddCheckers();
            services.AddBackgammon();

            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseSpaStaticFiles();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GameHub>("/gameHub");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
