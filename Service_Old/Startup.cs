using CCG.AspNetCore.Common.Configuration;
using CCG.AspNetCore.Web.Authorization;
using DataModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SapService
{
    public class Startup
    {
        private readonly Container _container = new Container();
        private readonly MpcConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _config = Configuration.Get<MpcConfiguration>();
        }

        public IConfiguration Configuration { get; }



        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins(_config.Identity.Client)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "PaC API", Version = "v1" });

                options.AddSecurityDefinition("oauth2", new OAuth2Scheme
                {
                    Flow = "implicit",
                    AuthorizationUrl = $"{_config.Identity.Authority}/connect/authorize",
                    Scopes = new Dictionary<string, string> {
                        { _config.Identity.ClientScope, "MariTime Planning and Costing API" }
                    }
                });
                options.OperationFilter<AuthorizeCheckOperationFilter>();
                options.CustomSchemaIds(i => i.FullName);
            });

            services.AddMvc();

            StartupHelper.ConfigureServices(services, _container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        [Obsolete]
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env)
        {
            ConfigureContainer(app);

            app.UseCors("CorsPolicy");
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            _container.Verify();

            app
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("v1/swagger.json", "My API V1");

                    options.OAuthClientId(_config.Identity.ClientId);

                    options.OAuthAppName(_config.Identity.ClientId);

                })
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller=Home}/{action=Index}/{id?}");

                    routes.MapSpaFallbackRoute(
                        name: "catch-all",
                        defaults: new { controller = "App", action = "RedirectIndex" });
                })
            .UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Context.Response.Headers.Add("Expires", "-1");
                }
            });


        }

        private void ConfigureContainer(IApplicationBuilder app)
        {

            _container.Register<DbContext, SapDbContext>(Lifestyle.Scoped);
            _container.Register<SapDbContext>(Lifestyle.Scoped);

            _container.Register(() => _config, Lifestyle.Singleton);
            _container.Register(() => _config.Identity, Lifestyle.Singleton);

            var assembliesToScan = AppDomain.CurrentDomain.GetAssemblies().Where(e => e.FullName.Contains("Business")).ToArray();
            //StartupHelper.Configure<SapDbContext>(app, _container, assembliesToScan);
        }



    }

    public class MpcConfiguration
    {
        public MpcIdentityConfiguration Identity { get; set; }

        public string PrintUrl { get; set; }
    }

    public class MpcIdentityConfiguration : CcgAccountClientConfiguration
    {
        public string ClientId { get; set; }
        public string ClientScope { get; set; }

    }
}
