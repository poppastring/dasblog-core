using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using DasBlog.SmokeTest.Interfaces;

namespace DasBlog.SmokeTest
{
	internal class WebServerRunner : IWebServerRunner
	{
		public void RunDasBlog()
		{
			ThreadStart runner = new ThreadStart(Start);
			Thread thr = new Thread(runner);
			thr.Start();
		}

		private void Start()
		{
			Process pr  = new Process();
			pr.StartInfo.UseShellExecute = true;
			pr.StartInfo.CreateNoWindow = false;
			pr.StartInfo.FileName = "dotnet";
			pr.StartInfo.Arguments = @"bin/debug/netcoreapp2.1/DasBlog.Web.dll";
			pr.StartInfo.WorkingDirectory = "c:/projects/dasblog-core/source/DasBlog.Web.UI/";
			pr.Start();
			pr.WaitForExit();
		}
	}
}
