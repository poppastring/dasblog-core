using AppConstants = DasBlog.Core.Common.Constants;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;


namespace DasBlog.Web.UI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}

		private static IWebHostEnvironment env;

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, configBuilder) =>
				{
					env = hostingContext.HostingEnvironment;

					configBuilder.AddXmlFile(Path.Combine(env.ContentRootPath, "Config", $"site.{env.EnvironmentName}.config"), optional: true, reloadOnChange: true)
						.AddXmlFile(Path.Combine(env.ContentRootPath, "Config", $"meta.{env.EnvironmentName}.config"), optional: true, reloadOnChange: true)
						.AddJsonFile(Path.Combine(env.ContentRootPath, $"appsettings.{env.EnvironmentName}.json"), optional: true)
						.AddEnvironmentVariables();

					MaybeOverrideRootUrl(configBuilder);
					configBuilder.Build();
				})
				.ConfigureLogging(loggingBuilder =>
				{
					loggingBuilder.AddFile(opts => opts.LogDirectory = Path.Combine(env.ContentRootPath, "Logs"));
				})
				.UseStartup<Startup>();

		private static void MaybeOverrideRootUrl(IConfigurationBuilder configBuilder)
		{
			if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(AppConstants.DasBlogOverrideRootUrl))
			  || Environment.GetEnvironmentVariable(AppConstants.DasBlogOverrideRootUrl)?.Trim() != "1")
			{
				return;
			}

			var urlsEnvVar = System.Environment.GetEnvironmentVariable(AppConstants.AspNetCoreUrls);
			if (string.IsNullOrWhiteSpace(urlsEnvVar))
			{
				urlsEnvVar = "http://*:5000/";
			}

			configBuilder.AddInMemoryCollection(new KeyValuePair<string, string>[]
				{new KeyValuePair<string, string>(nameof(ISiteConfig.Root), urlsEnvVar)});
		}
	}
}
