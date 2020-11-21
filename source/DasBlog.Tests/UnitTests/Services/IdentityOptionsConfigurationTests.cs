using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class IdentityOptionsConfigurationTests
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void LoadFromConfig()
		{
			var expected = new IdentityOptions
			{
				Password =
				{
					RequireDigit = true,
					RequiredLength = 8,
					RequireNonAlphanumeric = false,
					RequireUppercase = true,
					RequireLowercase = false,
					RequiredUniqueChars = 6
				},
				Lockout =
				{
					DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30),
					MaxFailedAccessAttempts = 10,
					AllowedForNewUsers = true
				},
				User =
				{
					RequireUniqueEmail = true
				}
			};

			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json")
				.Build();

			var candidate = new IdentityOptions();
			config.GetSection("IdentityOptions").Bind(candidate);

			// Pain in xUnit - see NCheck for nicer syntax :-)
			Assert.Equal(expected.Password.RequireDigit, candidate.Password.RequireDigit);
			Assert.Equal(expected.Password.RequiredLength, candidate.Password.RequiredLength);
			Assert.Equal(expected.Password.RequireNonAlphanumeric, candidate.Password.RequireNonAlphanumeric);
			Assert.Equal(expected.Password.RequireUppercase, candidate.Password.RequireUppercase);
			Assert.Equal(expected.Password.RequireLowercase, candidate.Password.RequireLowercase);
			Assert.Equal(expected.Password.RequiredUniqueChars, candidate.Password.RequiredUniqueChars);

			Assert.Equal(expected.Lockout.DefaultLockoutTimeSpan, candidate.Lockout.DefaultLockoutTimeSpan);
			Assert.Equal(expected.Lockout.MaxFailedAccessAttempts, candidate.Lockout.MaxFailedAccessAttempts);
			Assert.Equal(expected.Lockout.AllowedForNewUsers, candidate.Lockout.AllowedForNewUsers);

			Assert.Equal(expected.User.RequireUniqueEmail, candidate.User.RequireUniqueEmail);
		}
	}
}
