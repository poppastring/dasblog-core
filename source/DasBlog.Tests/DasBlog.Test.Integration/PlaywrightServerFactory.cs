using System;
using System.Linq;
using DasBlog.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace DasBlog.Test.Integration
{
	public class PlaywrightServerFactory<TStartup> : WebApplicationFactory<Startup> where TStartup : class
	{
		IWebHost _host;
		public string RootUri { get; set; }

		public PlaywrightServerFactory()
		{
			if (AreWe.InDockerOrBuildServer) return;

			ClientOptions.BaseAddress = new Uri("https://localhost");
		}

		protected override TestServer CreateServer(IWebHostBuilder builder)
		{
			_host = builder.Build();
			_host.Start();
			RootUri = _host.ServerFeatures.Get<IServerAddressesFeature>().Addresses.LastOrDefault();

			return new TestServer(new WebHostBuilder().UseStartup<TStartup>());
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_host?.Dispose();
			}
		}
	}
}
