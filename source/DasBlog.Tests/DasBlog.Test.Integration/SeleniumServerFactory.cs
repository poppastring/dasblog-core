using System;
using System.Diagnostics;
using System.Linq;
using DasBlog.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace DasBlog.Test.Integration
{
	public class SeleniumServerFactory<TStartup> : WebApplicationFactory<Startup> where TStartup : class
	{
		IWebHost _host;
		public string RootUri { get; set; }
		Process _process;

		public SeleniumServerFactory()
		{
			if (AreWe.InDockerOrBuildServer) return;

			ClientOptions.BaseAddress = new Uri("https://localhost"); //will follow redirects by default

			_process = new Process()
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "selenium-standalone",
					Arguments = "start",
					UseShellExecute = true,
					CreateNoWindow = false
				}
			};
			_process.Start();
		}

		// protected override void ConfigureWebHost(IWebHostBuilder builder)
		// {
		//     builder.UseEnvironment("Development"); //will be default in RC1
		//     //builder.UseKestrel();
		// }

		protected override TestServer CreateServer(IWebHostBuilder builder)
		{
			//Real TCP port
			_host = builder.Build();
			_host.Start();
			RootUri = _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault(); //Last is ssl!

			//Fake Server we won't use
			return new TestServer(new WebHostBuilder().UseStartup<TStartup>());
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_host?.Dispose();
				_process?.CloseMainWindow();
			}
		}
	}
}
