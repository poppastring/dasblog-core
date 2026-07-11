using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.Core.Security;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Services.Users;
using DasBlog.Web.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web.Identity
{
	public class DasBlogUserStoreTwoFactorTests
	{
		[Fact]
		public async Task ReplaceCodesAsync_StoresHashedCodesAndRedeemIsSingleUse()
		{
			var store = BuildStore(out _, out _);
			var user = new DasBlogUser { Email = "admin@example.com" };

			await store.ReplaceCodesAsync(user, new[] { "alpha-code", "bravo-code" }, CancellationToken.None);

			Assert.Equal(2, await store.CountCodesAsync(user, CancellationToken.None));
			Assert.DoesNotContain(user.RecoveryCodes, code => code.Hash == "alpha-code" || code.Hash == "bravo-code");
			Assert.All(user.RecoveryCodes, code => Assert.StartsWith("SHA256:", code.Hash));

			Assert.True(await store.RedeemCodeAsync(user, "alpha-code", CancellationToken.None));
			Assert.False(await store.RedeemCodeAsync(user, "alpha-code", CancellationToken.None));
			Assert.Equal(1, await store.CountCodesAsync(user, CancellationToken.None));
		}

		[Fact]
		public async Task SetTwoFactorEnabledAsync_DisableClearsSecretAndRecoveryCodes()
		{
			var store = BuildStore(out var users, out _);
			var user = new DasBlogUser
			{
				Email = "admin@example.com",
				TwoFactorEnabled = true,
				AuthenticatorSecret = "SECRET",
				RecoveryCodes = new List<RecoveryCode> { new RecoveryCode { Hash = "SHA256:HASH", Used = false } }
			};

			await store.SetTwoFactorEnabledAsync(user, false, CancellationToken.None);

			Assert.False(users[0].TwoFactor.Enabled);
			Assert.Null(users[0].TwoFactor.AuthenticatorSecret);
			Assert.Empty(users[0].TwoFactor.RecoveryCodes);
		}

		private static DasBlogUserStore BuildStore(out List<User> users, out Mock<ISiteSecurityConfig> securityConfig)
		{
			users = new List<User>
			{
				new User
				{
					EmailAddress = "admin@example.com",
					DisplayName = "Admin",
					Password = "HASH",
					TwoFactor = new TwoFactorSettings()
				}
			};

			var currentUsers = users;
			var userService = new Mock<IUserService>();
			userService.Setup(s => s.GetAllUsers()).Returns(() => currentUsers);
			userService.Setup(s => s.SaveUsers(It.IsAny<List<User>>()))
				.Callback<List<User>>(saved => currentUsers = saved);

			securityConfig = new Mock<ISiteSecurityConfig>();
			securityConfig.SetupProperty(c => c.Users, currentUsers);

			var settings = new Mock<IDasBlogSettings>();
			settings.Setup(s => s.SecurityConfiguration).Returns(securityConfig.Object);

			return new DasBlogUserStore(
				settings.Object,
				mapper: null,
				userService.Object,
				Mock.Of<ILogger<DasBlogUserStore>>());
		}
	}
}
