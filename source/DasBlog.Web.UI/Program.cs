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
			})
			.ConfigureLogging(loggingBuilder =>
			{
				loggingBuilder.AddFile(opts => opts.LogDirectory = Path.Combine(env.ContentRootPath, "Logs"));
			})
			.UseStartup<Startup>();
	}
}
