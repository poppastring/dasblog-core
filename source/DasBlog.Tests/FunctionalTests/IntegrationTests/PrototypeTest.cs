using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DasBlog.Tests.FunctionalTests.IntegrationTests
{
	public class PrototypeTest : IClassFixture<WebApplicationFactory<DasBlog.Web.Startup>>
	{
		private WebApplicationFactory<DasBlog.Web.Startup> webAppFactory;
		public PrototypeTest(WebApplicationFactory<DasBlog.Web.Startup> webAppFactory)
		{
			this.webAppFactory = webAppFactory;
		}
		[Fact]
		public void Minimal()
		{
			webAppFactory.CreateClient();
			Assert.True(true);
		}
	}
}
