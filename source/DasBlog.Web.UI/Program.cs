using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.UI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			CreateWebHostBuilder(args).Build().Run();
		}
		// asp.net core mvc testing insists that a CreateWebHostBuild named method exists
		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
			WebHost.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((hostingContext, configBuilder) =>
				{
					var env = hostingContext.HostingEnvironment;
					// tried setting the IConfigurationBuilder.BasePath to Startup.GetDataRoot
					// but this caused the view engine to go looking there for the views and fail to find them
					// Surely not what is supposed to happen?
					configBuilder.AddXmlFile(Path.Combine(Startup.GetDataRoot(env),@"Config/site.config"), optional: true, reloadOnChange: true)
						.AddXmlFile(Path.Combine(Startup.GetDataRoot(env),@"Config/metaConfig.xml"), optional: true, reloadOnChange: true)
						.AddJsonFile(Path.Combine(Startup.GetDataRoot(env),"appsettings.json"), optional: true, reloadOnChange: true)
						.AddJsonFile(Path.Combine(Startup.GetDataRoot(env),$"appsettings.{env.EnvironmentName}.json"), optional: true)
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
				.UseStartup<Startup>();

	}
}
