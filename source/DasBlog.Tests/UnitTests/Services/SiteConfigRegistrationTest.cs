using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class SiteConfigRegistrationTest
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void SiteConfig_ResolvesCurrentMonitoredValue()
		{
			var current = new SiteConfig { Title = "Original title" };
			var monitor = new Mock<IOptionsMonitor<SiteConfig>>();
			monitor.SetupGet(options => options.CurrentValue).Returns(() => current);

			var environment = new Mock<IWebHostEnvironment>();
			environment.SetupGet(env => env.ContentRootFileProvider).Returns(new NullFileProvider());

			var services = new ServiceCollection();
			services.AddSingleton(monitor.Object);
			services.AddDasBlogServices(environment.Object);

			using var serviceProvider = services.BuildServiceProvider();

			var original = serviceProvider.GetRequiredService<ISiteConfig>();
			current = new SiteConfig { Title = "Updated title" };
			var updated = serviceProvider.GetRequiredService<ISiteConfig>();

			Assert.Equal("Original title", original.Title);
			Assert.Equal("Updated title", updated.Title);
			Assert.NotSame(original, updated);
		}
	}
}
