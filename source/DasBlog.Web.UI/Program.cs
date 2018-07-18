using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Core.XmlRpc.Blogger;
using DasBlog.Core.Common;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
	            .ConfigureAppConfiguration((hostingContext, configBuilder) =>
	            {
		            var env = hostingContext.HostingEnvironment;
			        configBuilder.SetBasePath(env.ContentRootPath)
			            .AddXmlFile(@"Config\site.config", optional: true, reloadOnChange: true)
			            .AddXmlFile(@"Config\metaConfig.xml", optional: true, reloadOnChange: true)
			            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
			            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
			            .AddEnvironmentVariables()
			            ;
		            configBuilder.Build();
	            })
	            .ConfigureLogging(loggingBuilder =>
	            {
		            loggingBuilder.AddFile();
	            })
                .UseStartup<Startup>()
		        
                .Build();
    }
}
