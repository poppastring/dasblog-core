using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace DasBlog.Tests.FunctionalTests.IntegrationTests.Support
{
	public class DasBlogTestWebApplicationFactory
	  : WebApplicationFactory<DasBlog.Web.Startup>
	{
		public IServiceCollection Services { get; private set; }
		public IWebHost host { get; private set; }
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);
		}
	}
}
