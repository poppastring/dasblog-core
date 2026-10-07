using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class ConfigurationFileServicesTest : IDisposable
	{
		private readonly string tempDirectory;
		private readonly ConfigFilePathsDataOption paths;
		private readonly IAtomicFileWriter atomicFileWriter = new AtomicFileWriter();

		public ConfigurationFileServicesTest()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDirectory);
			paths = new ConfigFilePathsDataOption
			{
				MetaConfigFilePath = Path.Combine(tempDirectory, "meta.config"),
				OEmbedProvidersFilePath = Path.Combine(tempDirectory, "oembed-providers.json"),
				SecurityConfigFilePath = Path.Combine(tempDirectory, "siteSecurity.config")
			};
		}

		[Fact]
		public void SaveConfig_WritesValidMetadataXml()
		{
			var service = new MetaConfigFileService(Options.Create(paths),
				NullLogger<MetaConfigFileService>.Instance, atomicFileWriter);

			service.SaveConfig(new MetaTags { MetaDescription = "Atomic metadata" });

			var document = XDocument.Load(paths.MetaConfigFilePath);
			Assert.Equal("Atomic metadata", document.Root?.Element("MetaDescription")?.Value);
		}

		[Fact]
		public void SaveConfig_WritesValidOEmbedJson()
		{
			var service = new OEmbedProvidersFileService(Options.Create(paths),
				NullLogger<OEmbedProvidersFileService>.Instance, atomicFileWriter);

			service.SaveConfig(new OEmbedProviders { Providers = new List<OEmbedProvider>() });

			using var document = JsonDocument.Parse(File.ReadAllText(paths.OEmbedProvidersFilePath));
			Assert.Equal(JsonValueKind.Array, document.RootElement.GetProperty("providers").ValueKind);
		}

		[Fact]
		public void SaveConfig_WritesValidSecurityXml()
		{
			var service = new SiteSecurityConfigFileService(Options.Create(paths),
				NullLogger<SiteSecurityConfigFileService>.Instance, atomicFileWriter);

			service.SaveConfig(new SiteSecurityConfigData());

			var document = XDocument.Load(paths.SecurityConfigFilePath);
			Assert.NotNull(document.Root);
		}

		public void Dispose()
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}
}
