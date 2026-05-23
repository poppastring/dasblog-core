﻿using System.Collections.Generic;
using DasBlog.Core.Security;
using DasBlog.Managers.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web.Services
{
	public class DefaultCredentialsCheckTests
	{
		private const string IdentityHashOfAdmin = "AQAAAAIAAYagAAAAEH3HFD0v2zM73SHvLLO6+wXyhqIcsmEC2T1lwIkqJzJL4cT8M5h0K3PYV0L9aGRrYg==";

		private static IDefaultCredentialsCheck CreateCheck(IEnumerable<User> users, bool isDevelopment = false)
		{
			var userService = new Mock<IUserService>();
			userService.Setup(s => s.GetAllUsers()).Returns(users);

			var environment = new Mock<IHostEnvironment>();
			environment.SetupGet(e => e.EnvironmentName).Returns(isDevelopment ? "Development" : "Production");

			// Use a real SiteSecurityManager so VerifyHashedPassword behaves authentically.
			var settings = new Mock<DasBlog.Services.IDasBlogSettings>().Object;
			var security = new DasBlog.Managers.SiteSecurityManager(settings, new PasswordHasher<object>());

			return new DefaultCredentialsCheck(userService.Object, environment.Object, security);
		}

		[Fact]
		public void IsUsingDefaults_TrueForDefaultEmail()
		{
			var check = CreateCheck(new[]
			{
				new User { EmailAddress = "myemail@myemail.com", Password = "irrelevant" }
			});

			Assert.True(check.IsUsingDefaults());
		}

		[Fact]
		public void IsUsingDefaults_TrueForEmptyPassword()
		{
			var check = CreateCheck(new[]
			{
				new User { EmailAddress = "real@example.com", Password = string.Empty }
			});

			Assert.True(check.IsUsingDefaults());
		}

		[Fact]
		public void IsUsingDefaults_TrueForAdminStoredAsIdentityHash()
		{
			var hasher = new PasswordHasher<object>();
			var adminHash = hasher.HashPassword(null, "admin");

			var check = CreateCheck(new[]
			{
				new User { EmailAddress = "real@example.com", Password = adminHash }
			});

			Assert.True(check.IsUsingDefaults());
		}

		[Fact]
		public void IsUsingDefaults_TrueForAdminStoredAsLegacyMd5()
		{
			// Legacy BitConverter-formatted MD5(UTF-16LE) of "admin"
			const string legacyMd5OfAdmin = "19-A2-85-41-44-B6-3A-8F-76-17-A6-F2-25-01-9B-12";

			var check = CreateCheck(new[]
			{
				new User { EmailAddress = "real@example.com", Password = legacyMd5OfAdmin }
			});

			Assert.True(check.IsUsingDefaults());
		}

		[Fact]
		public void IsUsingDefaults_FalseForRealCredentials()
		{
			var hasher = new PasswordHasher<object>();
			var strongHash = hasher.HashPassword(null, "a-much-stronger-password-1!");

			var check = CreateCheck(new[]
			{
				new User { EmailAddress = "real@example.com", Password = strongHash }
			});

			Assert.False(check.IsUsingDefaults());
		}

		[Fact]
		public void IsUsingDefaults_FalseInDevelopmentEvenWithDefaults()
		{
			var check = CreateCheck(new[]
			{
				new User { EmailAddress = "myemail@myemail.com", Password = string.Empty }
			}, isDevelopment: true);

			Assert.False(check.IsUsingDefaults());
		}
	}
}
