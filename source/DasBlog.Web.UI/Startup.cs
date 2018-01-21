using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Rewrite;
using DasBlog.Web.UI.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using DasBlog.Web.UI.ViewsEngine;
using DasBlog.Web.Core;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using DasBlog.Web.Repositories.Interfaces;
using DasBlog.Web.Repositories;
using Microsoft.Extensions.FileProviders;
using System.Reflection;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.UI.Settings;
using DasBlog.Web.Core.Security;

namespace DasBlog.Web.UI
{
    public class Startup
    {
        private IHostingEnvironment _hostingEnvironment;

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddXmlFile(@"Config\site.config", optional: true, reloadOnChange: true)
            .AddXmlFile(@"Config\metaConfig.xml", optional: true, reloadOnChange: true)
            .AddXmlFile(@"Config\siteSecurity.config", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            Configuration = builder.Build();
            
            _hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            services.AddMemoryCache();

            services.Configure<SiteConfig>(Configuration);
            services.Configure<MetaTags>(Configuration);
            services.Configure<SiteSecurityConfig>(Configuration);
            services.AddTransient<IDasBlogSettings, DasBlogSettings>();

            services.Configure<RazorViewEngineOptions>(rveo => {
                rveo.ViewLocationExpanders.Add(new DasBlogLocationExpander(Configuration.GetSection("DasBlogSettings")["Theme"]));
            });

            services.AddSingleton<IBlogRepository, BlogRepository>();
            services.AddSingleton<ISubscriptionRepository, SubscriptionRepository>();
            services.AddSingleton<IArchiveRepository, ArchiveRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<ISiteRepository, SiteRepository>();
            services.AddSingleton<ISiteSecurityRepository, SiteSecurityRepository>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IPrincipal>(provider =>
                        provider.GetService<IHttpContextAccessor>().HttpContext.User);

            services.AddMvc();
            services.AddMvc().AddXmlSerializerFormatters();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    "Original Post Format",
                    "{posttitle}.aspx",                           
                    new { controller = "Home", action = "Post", posttitle = "" });

                routes.MapRoute(
                    "New Post Format",
                    "{posttitle}",
                    new { controller = "Home", action = "Post", id = "" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            RewriteOptions options = new RewriteOptions()
                 .AddIISUrlRewrite(env.ContentRootFileProvider, @"Config\IISUrlRewrite.xml");

            app.UseRewriter(options);
        }
    }
}
