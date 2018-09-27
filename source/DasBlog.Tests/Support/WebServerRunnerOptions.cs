using System.IO;

namespace DasBlog.Tests.Support
{
	public class WebServerRunnerOptions
	{
		public string WebServerExe { get; set; } = "dotnet";

		public string WorkingDirectory { get; set; }
		  = Path.Combine(Path.GetDirectoryName(typeof(WebServerRunnerOptions).Assembly.Location)
		  , "../../../../../DasBlog.Web.UI");
		public string WebServerProgramArguments { get; set; } = @"bin/debug/netcoreapp2.1/DasBlog.Web.dll";
	}
}
