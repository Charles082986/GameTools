using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlackFolderGames.Web.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WorldBuilder.Data;

namespace BlackFolderGames.Web
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddSingleton(_ => Configuration);


            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("Authentication")));

            services.AddDefaultIdentity<IdentityUser>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddAuthentication().AddGoogle(g =>
            {
                g.ClientId = Configuration["BlackFolderGames:Authentication:Google:ClientId"];
                g.ClientSecret = Configuration["BlackFolderGames:Authentication:Google:ClientSecret"];
                string scopes = Configuration["BlackFolderGames:Authentication:Google:Scopes"];
                if (!string.IsNullOrEmpty(scopes))
                {
                    foreach (string scope in scopes.Split('|'))
                    {
                        g.Scope.Add(scope);
                    }
                }
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanEditClaims", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Issuer == "LOCAL AUTHORITY" &&
                            c.Type == "CanEditClaims")));

                options.AddPolicy("CanViewUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Issuer == "LOCAL AUTHORITY" &&
                            c.Type == "CanViewUsers")));

                options.AddPolicy("CanSuspendUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Issuer == "LOCAL AUTHORITY" &&
                            c.Type == "CanSuspendUsers")));

                options.AddPolicy("CanBanUsers", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim(c =>
                            c.Issuer == "LOCAL AUTHORITY" &&
                            c.Type == "CanBanUsers")));
            });

            services.AddTransient<IUserRepository, UserRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
        }
    }
}
