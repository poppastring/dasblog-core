using System;
using System.Linq;
using DasBlog.Web;
using DasBlog.Services.ConfigFile;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DasBlog.Test.Integration
{
	public class PlaywrightServerFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : class
	{
		private IHost _host;
		public string RootUri { get; set; }
		public IServiceProvider HostServices => _host?.Services;

		public PlaywrightServerFactory()
		{
			if (AreWe.InDockerOrBuildServer) return;

           ClientOptions.BaseAddress = new Uri("https://127.0.0.1");
		}

       public void EnsureStarted()
		{
			try
			{
				_ = Services;
			}
			catch (InvalidCastException)
			{
				// Expected: WebApplicationFactory internally casts IServer to TestServer,
				// but we use Kestrel for real browser-based Playwright tests.
				// The host is already created and started at this point.
			}
		}

		protected override IHost CreateHost(IHostBuilder builder)
		{
			builder.ConfigureWebHost(webHostBuilder =>
			{
				webHostBuilder.UseKestrel();
				webHostBuilder.UseUrls("https://127.0.0.1:0");
			});

			_host = builder.Build();
			_host.Start();

			var server = _host.Services.GetRequiredService<IServer>();
			RootUri = server.Features.Get<IServerAddressesFeature>().Addresses.LastOrDefault();

			// Update SiteConfig.Root to match the actual server URL
			var siteConfig = _host.Services.GetService<IOptionsMonitor<SiteConfig>>();
			if (siteConfig != null && !string.IsNullOrEmpty(RootUri))
			{
				siteConfig.CurrentValue.Root = RootUri;
			}

			return _host;
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
