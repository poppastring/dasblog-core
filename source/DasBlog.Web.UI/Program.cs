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
						// there is magic afoot:
						//§ inclusion of NetEscapades.Extensions.Logging.RollingFile (which provides the file logger)
						// is sufficent for the logging builder to do the ncessary plumbing.
						// Why can't we at least have a type parameter to give thowe who
						// come after a fighting chance of knowing what's going on.  It would
						// save me having to type this explanation!
	            })
                .UseStartup<Startup>()
		        
                .Build();
    }
}
