using System.Collections.Generic;
using DasBlog.Core.Security;
using Xunit;
using DasBlog.Core.Services;
using DasBlog.Core.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq;

namespace DasBlog.Tests.UnitTests.Services
{
	public class UserDataRepoTest
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void Load_OnStandardConfig_ReturnsContainedUser()
		{
			IUserDataRepo repo = new UserDataRepo(
			  new OptionsAccessor<LocalUserDataOptions>{ Value = 
			  new LocalUserDataOptions{Path = "..\\..\\..\\Config\\SiteSecurity.config"}});
			List<User> users = repo.LoadUsers().ToList();

			Assert.Single(users, u => u.Name == "myemail@myemail.com");		// email is switched in for name by design
		}
	}

	public class OptionsAccessor<T> : IOptions<T> where T : class, new()
	{
		public T Value { get; set; }
	}
}
