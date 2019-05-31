using System.Net;
using System.Net.Http;
using DasBlog.Services.Users;
using DasBlog.Tests.FunctionalTests.IntegrationTests.Support;
using Xunit;

namespace DasBlog.Tests.FunctionalTests.IntegrationTests
{
	public class PrototypeIntegrationTests : IClassFixture<DasBlogTestWebApplicationFactory>
	{
		private DasBlogTestWebApplicationFactory webAppFactory;
		private ServiceResolver serviceResolver = new ServiceResolver();
		private HttpClient client;
		public PrototypeIntegrationTests(DasBlogTestWebApplicationFactory webAppFactory)
		{
			this.webAppFactory = webAppFactory;
			client = webAppFactory.CreateClient();
					// nothing happens (DI etc) until this has been called
		}
		[Fact(Skip="")]
		public async void MinimalTest()
		{
			var response = await client.GetAsync("http://localhost:5000/account/login");
			Assert.Equal( HttpStatusCode.OK, response.StatusCode);
		}

		[Fact(Skip="")]
		public void UsersTest()
		{
			var userService = serviceResolver.GetService<IUserService>();
			(var found, var user) = userService.FindMatchingUser("myemail@myemail.com");
			Assert.True(found);
		}
	}
}
