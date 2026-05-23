﻿using System.Collections.Generic;
using DasBlog.Core.Security;
using DasBlog.Services.Users;
using DasBlog.Web.Services;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web.Services
{
	public class FirstRunServiceTests
	{
		private static IFirstRunService Create(IEnumerable<User> users)
		{
			var userService = new Mock<IUserService>();
			userService.Setup(s => s.GetAllUsers()).Returns(users);
			return new FirstRunService(userService.Object);
		}

		[Fact]
		public void IsSetupRequired_TrueWhenNoUsers()
		{
			var service = Create(new List<User>());
			Assert.True(service.IsSetupRequired());
		}

		[Fact]
		public void IsSetupRequired_TrueWhenAllPasswordsEmpty()
		{
			var service = Create(new[]
			{
				new User { EmailAddress = "a@example.com", Password = string.Empty },
				new User { EmailAddress = "b@example.com", Password = "   " }
			});

			Assert.True(service.IsSetupRequired());
		}

		[Fact]
		public void IsSetupRequired_FalseWhenAnyUserHasPassword()
		{
			var service = Create(new[]
			{
				new User { EmailAddress = "a@example.com", Password = string.Empty },
				new User { EmailAddress = "b@example.com", Password = "hashed" }
			});

			Assert.False(service.IsSetupRequired());
		}

		[Fact]
		public void IsSetupRequired_TrueWhenUsersIsNull()
		{
			var userService = new Mock<IUserService>();
			userService.Setup(s => s.GetAllUsers()).Returns((IEnumerable<User>)null);
			var service = new FirstRunService(userService.Object);

			Assert.True(service.IsSetupRequired());
		}
	}
}
