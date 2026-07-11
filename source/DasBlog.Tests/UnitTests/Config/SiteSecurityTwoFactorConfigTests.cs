using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DasBlog.Core.Security;
using DasBlog.Services.ConfigFile;
using Xunit;

namespace DasBlog.Tests.UnitTests.Config
{
	public class SiteSecurityTwoFactorConfigTests
	{
		[Fact]
		public void Serialize_UserWithoutTwoFactor_OmitsTwoFactorElement()
		{
			var config = new SiteSecurityConfigData
			{
				Users = new List<User>
				{
					new User
					{
						Role = Role.Admin,
						EmailAddress = "admin@example.com",
						DisplayName = "Admin",
						Active = true,
						Password = "HASH"
					}
				}
			};

			var xml = Serialize(config);

			Assert.DoesNotContain("<TwoFactor", xml);
		}

		[Fact]
		public void Deserialize_OldConfigWithoutTwoFactor_LoadsWithDisabledTwoFactor()
		{
			const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SiteSecurityConfig>
  <Users>
	<User>
	  <Role>Admin</Role>
	  <EmailAddress>admin@example.com</EmailAddress>
	  <DisplayName>Admin</DisplayName>
	  <Active>true</Active>
	  <Password>HASH</Password>
	</User>
  </Users>
</SiteSecurityConfig>";

			var config = Deserialize(xml);

			Assert.Single(config.Users);
			Assert.False(config.Users[0].TwoFactor.Enabled);
			Assert.Null(config.Users[0].TwoFactor.AuthenticatorSecret);
			Assert.Empty(config.Users[0].TwoFactor.RecoveryCodes);
		}

		[Fact]
		public void Deserialize_TwoFactorConfig_LoadsSecretAndRecoveryCodes()
		{
			const string xml = @"<?xml version=""1.0"" encoding=""utf-8""?>
<SiteSecurityConfig>
  <Users>
	<User>
	  <Role>Admin</Role>
	  <EmailAddress>admin@example.com</EmailAddress>
	  <DisplayName>Admin</DisplayName>
	  <Active>true</Active>
	  <Password>HASH</Password>
	  <TwoFactor enabled=""true"">
		<AuthenticatorSecret>SECRET</AuthenticatorSecret>
		<RecoveryCodes>
		  <Code hash=""SHA256:ONE"" used=""false"" />
		  <Code hash=""SHA256:TWO"" used=""true"" />
		</RecoveryCodes>
	  </TwoFactor>
	</User>
  </Users>
</SiteSecurityConfig>";

			var config = Deserialize(xml);
			var twoFactor = config.Users[0].TwoFactor;

			Assert.True(twoFactor.Enabled);
			Assert.Equal("SECRET", twoFactor.AuthenticatorSecret);
			Assert.Equal(2, twoFactor.RecoveryCodes.Count);
			Assert.Equal("SHA256:ONE", twoFactor.RecoveryCodes[0].Hash);
			Assert.False(twoFactor.RecoveryCodes[0].Used);
			Assert.True(twoFactor.RecoveryCodes[1].Used);
		}

		private static string Serialize(SiteSecurityConfigData config)
		{
			var serializer = new XmlSerializer(typeof(SiteSecurityConfigData));
			using var writer = new StringWriter();
			serializer.Serialize(writer, config);
			return writer.ToString();
		}

		private static SiteSecurityConfigData Deserialize(string xml)
		{
			var serializer = new XmlSerializer(typeof(SiteSecurityConfigData));
			using var reader = new StringReader(xml);
			return (SiteSecurityConfigData)serializer.Deserialize(reader);
		}
	}
}
