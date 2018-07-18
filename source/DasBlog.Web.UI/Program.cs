using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Core.XmlRpc.Blogger;
using DasBlog.Web.Common;
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
            WebHost.CreateDefaultBuilder(args.Where(a => !a.ToLower().StartsWith(Constants.__DIAGNOSE_LOGGING_ISSUES)).ToArray())
							// command line args prefixed by "--" have to be "registered" as command line switches
							// in the AddCommandLine call.  This appears to demand a key-value pair which
							// we don't want.  So next best thing is not to pass that arg to the builder
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
		            if (args.Any(a => a.ToLower().StartsWith(Constants.__DIAGNOSE_LOGGING_ISSUES)))
		            {
			            // just remove the "default" settings from appSettings.json and appSettings-dee...json
			            // and do dotnet run --diagnose-logging-issues or equivalent
			            // break here to see all our logging not covered by explicit rules
			            loggingBuilder.AddFilter("DasBlog", logLevel => { return true; });
		            }
	            })
                .UseStartup<Startup>()
		        
                .Build();
    }
}
