using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DasBlog.SmokeTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var host = new HostBuilder()
				.ConfigureAppConfiguration((hostContext, builder) =>
				{
					builder.AddCommandLine(args
						, new Dictionary<string, string>
						{
							{"--contentRoot", ""}
						});
				}).ConfigureServices((hostContext, services) =>
				{
					services.AddSingleton<IVersionedFileService, GitVersionedFileService>();
					services.AddSingleton<IDasBlogInstallation, DasBlogInstallation>();
					services.AddSingleton<IWebRunner, WebRunner>();
					services.AddHostedService<SmokeTester>();
					services.AddSingleton<IBrowserFacade, BrowserFacade>();
				}).ConfigureLogging((hostContext, logBuilder) =>
				{
					logBuilder.AddConsole();
					logBuilder.AddDebug();
				}).Build();
			host.Run();
		}
	}

	internal class BrowserFacade : IBrowserFacade
	{
	}

	public interface IBrowserFacade
	{
	}

	internal class SmokeTester : IHostedService
	{
		public Task StartAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}

	internal class WebRunner : IWebRunner
	{
		public void RunDasBlog()
		{
			throw new NotImplementedException();
		}
	}

	public interface IWebRunner
	{
		void RunDasBlog();
	}

	internal class DasBlogInstallation : IDasBlogInstallation
	{
		public void Init()
		{
			throw new NotImplementedException();
		}

		public void Terminate()
		{
			throw new NotImplementedException();
		}

		public string GetConfigPathAndFile()
		{
			throw new NotImplementedException();
		}

		public string GetContentDirectoryPath()
		{
			throw new NotImplementedException();
		}

		public string GetLogDirectoryPath()
		{
			throw new NotImplementedException();
		}

		public string GetWwwRootDirectoryPath()
		{
			throw new NotImplementedException();
		}
	}

	public interface IDasBlogInstallation
	{
		void Init();
		void Terminate();
		string GetConfigPathAndFile();
		string GetContentDirectoryPath();
		string GetLogDirectoryPath();
		string GetWwwRootDirectoryPath();
	}

	internal class GitVersionedFileService : IVersionedFileService
	{
		public (bool active, string errorMessage) IsActive()
		{
			throw new NotImplementedException();
		}

		public (bool clean, string errorMessage) IsClean()
		{
			throw new NotImplementedException();
		}

		public void Restore()
		{
			throw new NotImplementedException();
		}
	}

	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();
		(bool clean, string errorMessage) IsClean();
		void Restore();
	}
}
