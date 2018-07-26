using DasBlog.Core.Services;
using DasBlog.Core.Services.Interfaces;
using Xunit;

namespace DasBlog.Tests.Services
{
	public class EventLineParserTest
	{
		[Fact]
		public void Parse_OnValidInput_ReturnsSuccess()
		{
			IEventLineParser parser = new EventLineParser();
			(bool success, var _) = parser.Parse(
			  @"2018-07-18 14:38:08.591 +01:00 [Information] DasBlog.Web.Controllers.AccountController: SecuritySuccess :: myemail@myemail.com logged in successfully :: http://localhost:50432/Account/Login"
			  );
			Assert.True(success);
		}
		[Fact]
		public void Parse_OnInvalidInput_ReturnsFailure()
		{
			IEventLineParser parser = new EventLineParser();
			(bool success, var _) = parser.Parse(
			  @"2018-07-18 09:10:08.388 +01:00 [Information] Microsoft.AspNetCore.Authorization.DefaultAuthorizationService: Authorization failed for user: (null)."
			  );
			Assert.False(success);
		}
	}
}
