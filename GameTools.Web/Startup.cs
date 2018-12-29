using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameTools.Authentication.Application.AuthenticationProviders;
using GameTools.Authentication.Application.Services;
using GameTools.Authentication.Interfaces.AuthenticationProviders;
using GameTools.Authentication.Interfaces.Services;
using GameTools.Web.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GameTools.Web
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
            services.AddMvc();
            services.AddSingleton(_ => Configuration);
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IAuthenticationProviderFactory, AuthenticationProviderFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMiddleware<ClientIdInjector>();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public void Application_BeginReqeust()
        {

        }
    }
}
