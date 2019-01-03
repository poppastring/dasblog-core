using System;
using System.Diagnostics;
using System.Threading;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace DasBlog.Tests.Support
{
	public class WebServerRunner : IWebServerRunner
	{
		private string exe;
		private string args;
		private string workingDirectory;
		private Process webAppProcess;
		private ILogger<WebServerRunner> logger;

		public WebServerRunner(IOptions<WebServerRunnerOptions> optionsAccessor, ILogger<WebServerRunner> logger)
		{
			exe = optionsAccessor.Value.WebServerExe;
			args = optionsAccessor.Value.WebServerProgramArguments;
			workingDirectory = optionsAccessor.Value.WorkingDirectory;
			this.logger = logger;
		}
		public void RunDasBlog()
		{
			ThreadStart runner = new ThreadStart(Start);
			Thread thr = new Thread(runner);
			thr.Start();
		}

		public void Kill()
		{
			if (webAppProcess != null && !webAppProcess.HasExited)
			{
				webAppProcess.Kill();
			}
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
			webAppProcess = pr;
			string s;
			while ((s = pr.StandardOutput.ReadLine()) != null)
			{
				logger.LogInformation($"DasBlog.Web says: {s}");
			}
		}
	}
}
