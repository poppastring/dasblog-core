namespace DasBlog.SmokeTest
{
	public class WebServerRunnerOptions
	{
		public string WebServerExe { get; set; } = "dotnet";
		public string WorkingDirectory { get; set; } = "c:/projects/dasblog-core/source/DasBlog.Web.UI/";
		public string WebServerProgramArguments { get; set; } = @"bin/debug/netcoreapp2.1/DasBlog.Web.dll";
	}
}
