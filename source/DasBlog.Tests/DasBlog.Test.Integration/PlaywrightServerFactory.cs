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
	public class PlaywrightServerFactory<TEntryPoint> : WebApplicationFactory<Program> where TEntryPoint : class
	{
		IWebHost _host;
		public string RootUri { get; set; }

		public PlaywrightServerFactory()
		{
			if (AreWe.InDockerOrBuildServer) return;

			ClientOptions.BaseAddress = new Uri("https://localhost");
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
