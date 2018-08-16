using DasBlog.Tests.Automation.Selenium;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using DasBlog.Tests.FunctionalTests.IntegrationTests.Support;
using Xunit;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.FunctionalTests
{
	public class BrowserOptionsAccessor : IOptions<BrowserOptions>
	{
		public BrowserOptionsAccessor(BrowserOptions opts)
		{
			Value = opts;
		}
		public BrowserOptions Value { get; }
	}
	public class PrototypeBrowserBasedTests : IClassFixture<DasBlogTestWebApplicationFactory>
	{
		private DasBlogTestWebApplicationFactory webAppFactory;
		public PrototypeBrowserBasedTests(DasBlogTestWebApplicationFactory webApplicationFactory)
		{
			this.webAppFactory = webApplicationFactory;
			webAppFactory.CreateClient();
		}
		[Fact]
		public void MinimalTest()
		{
			IBrowser browser = new Browser(
			  new BrowserOptionsAccessor(new BrowserOptions
			  {
				  HomeUrl =  "http://localhost/",
				  Driver = "firefox"
			  }));
			browser.Init();
			browser.Goto("/account/login");
		}
	}
}
