using System.Text;
using Demo.Controllers;
using Demo.Infrastructure;
using Demo.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace Demo
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:App"])
            );

            //https://stackoverflow.com/questions/37371264/invalidoperationexception-unable-to-resolve-service-for-type-microsoft-aspnetc
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddTransient<IDataRepository, DataRepository>();
            services.AddTransient<IDataController, DataController>();

            services.AddCors();
            services.AddMvc();
            services.AddSession();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment() || env.EnvironmentName.Equals("Testing"))
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseStatusCodePagesWithReExecute("/Home/Error");
            }
            app.UseCors(builder =>
                builder.WithOrigins("*")
                       .AllowAnyHeader()
                       .AllowCredentials()
                       .AllowAnyMethod()
                       .WithHeaders()
                );
            app.UseStaticFiles();
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Home",
                    template: "",
                    defaults: new {controller="Home", action="Index"}
                );

                routes.MapRoute(
                    name: "Articles",
                    template: "Articles/{type}/{page}",
                    defaults: new { controller = "Article", action = "List", type = "{type}"    , page = "{page}" }
                );

                routes.MapRoute(
                    name: "Auth",
                    template: "Auth",
                    defaults: new { controller = "Auth", action = "Login" }
                );

                routes.MapRoute(
                    name: "Article Detail",
                    template: "{name}-{id}",
                    defaults: new { controller = "Article", action = "Read" }
                );

                routes.MapRoute(name: null, template: "{controller}/{action}/{id?}");
            });
            app.UseStatusCodePages();

            //Initialize.Begin(app);
        }
    }
}
