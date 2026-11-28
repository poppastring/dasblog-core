using System.IO;
using System.Text;
using DasBlog.Services.ConfigFile;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace DasBlog.Test.Integration
{
	public class MailProviderCompatibilityTests
	{
		[Theory]
		[InlineData("", MailProviderType.Custom)]
		[InlineData("<MailProvider>Custom</MailProvider>", MailProviderType.Custom)]
		[InlineData("<MailProvider>Brevo</MailProvider>", MailProviderType.Brevo)]
		public void Configuration_ExistingEnvironmentOverrides_PreservesProviderSelection(string providerXml, MailProviderType expected)
		{
			var sourceRoot = ThemeActivationRenderingTests.FindSourceRoot();
			var xml = $"<SiteConfig><SmtpServer>smtp.example.com</SmtpServer><SmtpPort>2525</SmtpPort>{providerXml}</SiteConfig>";
			using var environmentConfig = new MemoryStream(Encoding.UTF8.GetBytes(xml));
			using var configuration = (ConfigurationRoot)new ConfigurationBuilder()
				.SetBasePath(Path.Combine(sourceRoot, "DasBlog.Web"))
				.AddXmlFile(@"Config\site.config", optional: false)
				.AddXmlStream(environmentConfig)
				.Build();

			var site = configuration.Get<SiteConfig>();

			Assert.Equal(expected, site.MailProvider);
			Assert.Equal("smtp.example.com", site.SmtpServer);
			Assert.Equal(2525, site.SmtpPort);
		}
	}
}
