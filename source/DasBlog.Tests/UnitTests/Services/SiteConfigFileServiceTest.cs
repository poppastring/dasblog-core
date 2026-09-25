using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class SiteConfigFileServiceTest : IDisposable
	{
		private readonly string tempDirectory;
		private readonly string siteConfigPath;
		private readonly SiteConfigFileService service;

		public SiteConfigFileServiceTest()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDirectory);
			siteConfigPath = Path.Combine(tempDirectory, "site.config");
			service = new SiteConfigFileService(
				Options.Create(new ConfigFilePathsDataOption { SiteConfigFilePath = siteConfigPath }),
				NullLogger<SiteConfigFileService>.Instance);
		}

		[Fact]
		public async Task SaveConfig_DoesNotExposePartialXmlToConcurrentReaders()
		{
			service.SaveConfig(new SiteConfig { Theme = "dasblog", Description = "Initial configuration" });
			var largeDescription = new string('x', 8 * 1024 * 1024);
			var observedInvalidXml = false;

			for (var attempt = 0; attempt < 5 && !observedInvalidXml; attempt++)
			{
				var saveTask = Task.Run(() => service.SaveConfig(new SiteConfig
				{
					Theme = attempt % 2 == 0 ? "darkly" : "dasblog",
					Description = largeDescription
				}));

				while (!saveTask.IsCompleted)
				{
					try
					{
						using var stream = new FileStream(siteConfigPath, FileMode.Open, FileAccess.Read,
							FileShare.ReadWrite | FileShare.Delete);
						XDocument.Load(stream);
					}
					catch (XmlException)
					{
						observedInvalidXml = true;
					}
					catch (IOException)
					{
						// Retry while the writer is replacing or updating the file.
					}
				}

				await saveTask;
			}

			Assert.False(observedInvalidXml, "A concurrent reader observed a truncated or partially written site.config file.");
		}

		public void Dispose()
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}
}
