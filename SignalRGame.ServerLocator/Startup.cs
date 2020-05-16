using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SignalRGame.Discovery;

namespace SignalRGame.ServerLocator
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
            // TODO - detect kubernetes
            services.AddTransient<IServerDiscovery>(sp => new ConfigurationServerDiscovery((from gameServer in Configuration["GAME_SERVERS"].Split(';')
                                                                                                               let parts = gameServer.Split(',')
                                                                                                               select new ServerDetails(parts[0], parts[parts.Length == 1 ? 0: 1])).ToArray()));
            services.AddTransient<IGameServers, GameServers>();
            services.AddTransient<Clients.IGameClientFactory>(sp => sp.GetRequiredService<Clients.GameClientFactory>());

            services.AddHttpClient<Clients.GameClientFactory>();
            services.AddMemoryCache();
            services.AddControllers().AddNewtonsoftJson();

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

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
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
