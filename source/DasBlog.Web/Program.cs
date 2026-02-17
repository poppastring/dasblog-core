using Coravel;
using DasBlog.Services;
using DasBlog.Services.FileManagement;
using DasBlog.Services.Scheduler;
using DasBlog.Services.Site;
using DasBlog.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;

DasBlogPathResolver.EnsureDefaultConfigFiles(
	Directory.GetCurrentDirectory(),
	Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production");

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment;

var defaultSiteConfigPath = Path.Combine("Config", "site.config");
var siteConfigPath = Path.Combine("Config", $"site.{env.EnvironmentName}.config");
var defaultMetaConfigPath = Path.Combine("Config", "meta.config");
var metaConfigPath = Path.Combine("Config", $"meta.{env.EnvironmentName}.config");
var oembedProvidersPath = Path.Combine("Config", "oembed-providers.json");

builder.Configuration.Sources.Clear();
builder.Configuration
	.SetBasePath(env.ContentRootPath)
	.AddXmlFile(defaultSiteConfigPath, optional: false, reloadOnChange: true)
	.AddXmlFile(siteConfigPath, optional: true, reloadOnChange: true)
	.AddXmlFile(defaultMetaConfigPath, optional: false, reloadOnChange: true)
	.AddXmlFile(metaConfigPath, optional: true, reloadOnChange: true)
	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
	.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
	.AddJsonFile(oembedProvidersPath, optional: true, reloadOnChange: true)
	.AddEnvironmentVariables();

var pathResolver = new DasBlogPathResolver(env.ContentRootPath, env.EnvironmentName, builder.Configuration);

builder.Services.AddDasBlogConfiguration(builder.Configuration, pathResolver);
builder.Services.AddDasBlogSecurity(builder.Configuration);
builder.Services.AddDasBlogWebServices(builder.Configuration);

builder.Services.AddDasBlogDataServices();
builder.Services.AddDasBlogManagers();
builder.Services.AddDasBlogServices(env);

var app = builder.Build();

var siteRepairer = app.Services.GetRequiredService<ISiteRepairer>();
(var siteOk, var siteError) = siteRepairer.RepairSite();

if (env.IsDevelopment() || env.IsStaging())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/home/error");
}

if (env.IsStaging() || env.IsProduction())
{
	app.UseHsts(options => options.MaxAge(days: 30));
}

app.Services.UseScheduler(scheduler =>
{
	scheduler
		.Schedule<SiteEmailReport>()
		.DailyAt(23, 19)
		.Zoned(TimeZoneInfo.Local);
});

if (!siteOk)
{
	app.Run(async context => await context.Response.WriteAsync(siteError));
	return;
}

var paths = app.Services.GetRequiredService<IDasBlogPathResolver>();
var dasBlogSettings = app.Services.GetRequiredService<IDasBlogSettings>();

var rewriteOptions = new RewriteOptions()
	.AddIISUrlRewrite(env.ContentRootFileProvider, paths.IISUrlRewriteRelativePath);

app.UseRewriter(rewriteOptions);
app.UseRouting();

//if you've configured it at /blog or /whatever, set that pathbase so ~ will generate correctly
var basePath = "/";
if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.Root))
{
	var rootUri = new Uri(dasBlogSettings.SiteConfiguration.Root);
	basePath = rootUri.AbsolutePath;
}

//Deal with path base and proxies that change the request path
if (basePath != "/")
{
	app.Use((context, next) =>
	{
		context.Request.PathBase = new PathString(basePath);
		return next.Invoke();
	});
}

app.UseForwardedHeaders();
app.UseDasBlogStaticFiles(env, paths);
app.UseCookiePolicy();

app.UseAuthentication();
app.UseDasBlogThreadPrincipal();
app.UseAuthorization();

app.UseDasBlogSecurityHeaders(app.Configuration);
app.UseDasBlogEndpoints();

app.UseHttpContext();

app.Run();

namespace DasBlog.Web
{
	public partial class Program { }
}
