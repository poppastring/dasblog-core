using System;
using System.Linq;
using DasBlog.Web;
using DasBlog.Services.ConfigFile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

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

			// Update SiteConfig.Root to match the actual server URL
			var siteConfig = _host.Services.GetService<IOptionsMonitor<SiteConfig>>();
			if (siteConfig != null && !string.IsNullOrEmpty(RootUri))
			{
				siteConfig.CurrentValue.Root = RootUri;
			}

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
