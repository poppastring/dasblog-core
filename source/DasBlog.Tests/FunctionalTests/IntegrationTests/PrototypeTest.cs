using System.Net;
using System.Threading;
using DasBlog.Tests.FunctionalTests.IntegrationTests.Support;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DasBlog.Tests.FunctionalTests.IntegrationTests
{
	public class PrototypeTest : IClassFixture<DasBlogTestWebApplicationFactory>
	{
		private WebApplicationFactory<DasBlog.Web.Startup> webAppFactory;
		public PrototypeTest(DasBlogTestWebApplicationFactory webAppFactory)
		{
			this.webAppFactory = webAppFactory;
		}
		[Fact]
		public async void Minimal()
		{
			var client = webAppFactory.CreateClient();
			var response = await client.GetAsync("http://localhost:5000/");
			Assert.Equal( HttpStatusCode.Accepted, response.StatusCode);
		}
	}
}
