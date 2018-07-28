using System;
using System.Diagnostics;
using System.Threading;
using DasBlog.SmokeTest.Interfaces;
using Microsoft.Extensions.Options;

namespace DasBlog.SmokeTest
{
	internal class WebServerRunner : IWebServerRunner
	{
		private string exe;
		private string args;
		private string workingDirectory;
		public WebServerRunner(IOptions<WebServerRunnerOptions> optionsAccessor)
		{
			exe = optionsAccessor.Value.WebServerExe;
			args = optionsAccessor.Value.WebServerProgramArguments;
			workingDirectory = optionsAccessor.Value.WorkingDirectory;
		}
		public void RunDasBlog()
		{
			ThreadStart runner = new ThreadStart(Start);
			Thread thr = new Thread(runner);
			thr.Start();
		}

		private void Start()
		{
			Process pr  = new Process();
			pr.StartInfo.UseShellExecute = false;
			pr.StartInfo.CreateNoWindow = false;
			pr.StartInfo.FileName = exe; //"dotnet"
			pr.StartInfo.Arguments = args; //@"bin/debug/netcoreapp2.1/DasBlog.Web.dll"
			pr.StartInfo.WorkingDirectory = workingDirectory; //"c:/projects/dasblog-core/source/DasBlog.Web.UI/"
			pr.StartInfo.RedirectStandardOutput = true;
			pr.Start();
			string s;
			while ((s = pr.StandardOutput.ReadLine()) != null)
			{
				Console.WriteLine($"DasBlog.Web says: {s}");
			}
		}
	}
}
