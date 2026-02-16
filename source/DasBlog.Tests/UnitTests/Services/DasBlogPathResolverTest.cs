using System.IO;
using Xunit;
using DasBlog.Services.FileManagement;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace DasBlog.Tests.UnitTests.Services
{
	public class DasBlogPathResolverTest
	{
		private static string NormalizePath(string path) => Path.GetFullPath(path);

		private static IConfiguration BuildConfig(Dictionary<string, string> values = null)
		{
			var builder = new ConfigurationBuilder();
			builder.AddInMemoryCollection(values ?? new Dictionary<string, string>());
			return builder.Build();
		}

		[Fact]
		public void Constructor_SetsContentRootPath()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(root, resolver.ContentRootPath);
		}

		[Fact]
		public void RelativePaths_ContainEnvironmentName()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Staging", config);

			Assert.Equal(Path.Combine("Config", "site.Staging.config"), resolver.SiteConfigRelativePath);
			Assert.Equal(Path.Combine("Config", "meta.Staging.config"), resolver.MetaConfigRelativePath);
			Assert.Equal(Path.Combine("Config", "siteSecurity.Staging.config"), resolver.SecurityConfigRelativePath);
			Assert.Equal(Path.Combine("Config", "IISUrlRewrite.Staging.config"), resolver.IISUrlRewriteRelativePath);
		}

		[Fact]
		public void DefaultRelativePaths_DoNotContainEnvironmentName()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Production", config);

			Assert.Equal(Path.Combine("Config", "site.config"), resolver.DefaultSiteConfigRelativePath);
			Assert.Equal(Path.Combine("Config", "meta.config"), resolver.DefaultMetaConfigRelativePath);
			Assert.Equal(Path.Combine("Config", "siteSecurity.config"), resolver.DefaultSecurityConfigRelativePath);
			Assert.Equal(Path.Combine("Config", "IISUrlRewrite.config"), resolver.DefaultIISUrlRewriteRelativePath);
			Assert.Equal(Path.Combine("Config", "oembed-providers.json"), resolver.OEmbedProvidersRelativePath);
		}

		[Fact]
		public void AbsoluteConfigPaths_CombineRootAndRelative()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(Path.Combine(root, "Config", "site.Development.config"), resolver.SiteConfigFilePath);
			Assert.Equal(Path.Combine(root, "Config", "meta.Development.config"), resolver.MetaConfigFilePath);
			Assert.Equal(Path.Combine(root, "Config", "siteSecurity.Development.config"), resolver.SecurityConfigFilePath);
			Assert.Equal(Path.Combine(root, "Config", "IISUrlRewrite.Development.config"), resolver.IISUrlRewriteFilePath);
			Assert.Equal(Path.Combine(root, "Config", "oembed-providers.json"), resolver.OEmbedProvidersFilePath);
		}

		[Fact]
		public void ThemeFolderPath_CombinesRootAndTheme()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "mytheme" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(NormalizePath(Path.Combine(root, "Themes", "mytheme")), resolver.ThemeFolderPath);
		}

		[Fact]
		public void ThemeFolderPath_DefaultsToDasblog_WhenThemeNotConfigured()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(NormalizePath(Path.Combine(root, "Themes", "dasblog")), resolver.ThemeFolderPath);
		}

		[Fact]
		public void BinariesPath_CombinesRootAndBinariesDir()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(NormalizePath(Path.Combine(root, "content", "binary")), resolver.BinariesPath);
		}

		[Fact]
		public void BinariesUrlRelativePath_ReturnsContentBinary()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal("content/binary", resolver.BinariesUrlRelativePath);
		}

		[Fact]
		public void LogFolderPath_UsesRelativeDir_WhenNotRooted()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" },
				{ "LogDir", "logs" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(Path.Combine(root, "logs"), resolver.LogFolderPath);
		}

		[Fact]
		public void LogFolderPath_UsesAbsolutePath_WhenRooted()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var absoluteLogDir = NormalizePath(Path.Combine(Path.GetTempPath(), "absolute-logs"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" },
				{ "LogDir", absoluteLogDir }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(absoluteLogDir, resolver.LogFolderPath);
		}

		[Fact]
		public void LogFolderPath_DefaultsToLogs_WhenNotConfigured()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(Path.Combine(root, "logs"), resolver.LogFolderPath);
		}

		[Fact]
		public void ContentFolderPath_CombinesRootAndContentDir()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "site-content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(NormalizePath(Path.Combine(root, "site-content")), resolver.ContentFolderPath);
		}

		[Fact]
		public void RadioStoriesFolderPath_IsUnderContentRoot()
		{
			var root = NormalizePath(Path.Combine(Path.GetTempPath(), "myapp"));
			var config = BuildConfig(new Dictionary<string, string>
			{
				{ "Theme", "dasblog" },
				{ "BinariesDir", "content/binary" },
				{ "ContentDir", "content" }
			});

			var resolver = new DasBlogPathResolver(root, "Development", config);

			Assert.Equal(NormalizePath(Path.Combine(root, "content", "radioStories")), resolver.RadioStoriesFolderPath);
		}

		[Fact]
		public void EnsureDefaultConfigFiles_CopiesSecurityConfig_WhenEnvFileMissing()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Path.GetRandomFileName());
			var configDir = Path.Combine(tempDir, "Config");
			Directory.CreateDirectory(configDir);

			var defaultFile = Path.Combine(configDir, "siteSecurity.config");
			File.WriteAllText(defaultFile, "<security />");

			try
			{
				DasBlogPathResolver.EnsureDefaultConfigFiles(tempDir, "Staging");

				var envFile = Path.Combine(configDir, "siteSecurity.Staging.config");
				Assert.True(File.Exists(envFile));
				Assert.Equal("<security />", File.ReadAllText(envFile));
			}
			finally
			{
				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, recursive: true);
			}
		}

		[Fact]
		public void EnsureDefaultConfigFiles_CopiesIISRewriteConfig_WhenEnvFileMissing()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Path.GetRandomFileName());
			var configDir = Path.Combine(tempDir, "Config");
			Directory.CreateDirectory(configDir);

			var defaultFile = Path.Combine(configDir, "IISUrlRewrite.config");
			File.WriteAllText(defaultFile, "<rewrite />");

			try
			{
				DasBlogPathResolver.EnsureDefaultConfigFiles(tempDir, "Production");

				var envFile = Path.Combine(configDir, "IISUrlRewrite.Production.config");
				Assert.True(File.Exists(envFile));
				Assert.Equal("<rewrite />", File.ReadAllText(envFile));
			}
			finally
			{
				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, recursive: true);
			}
		}

		[Fact]
		public void EnsureDefaultConfigFiles_DoesNotOverwrite_WhenEnvFileExists()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Path.GetRandomFileName());
			var configDir = Path.Combine(tempDir, "Config");
			Directory.CreateDirectory(configDir);

			File.WriteAllText(Path.Combine(configDir, "siteSecurity.config"), "<default />");
			File.WriteAllText(Path.Combine(configDir, "siteSecurity.Staging.config"), "<existing />");
			File.WriteAllText(Path.Combine(configDir, "IISUrlRewrite.config"), "<default-iis />");
			File.WriteAllText(Path.Combine(configDir, "IISUrlRewrite.Staging.config"), "<existing-iis />");

			try
			{
				DasBlogPathResolver.EnsureDefaultConfigFiles(tempDir, "Staging");

				Assert.Equal("<existing />", File.ReadAllText(Path.Combine(configDir, "siteSecurity.Staging.config")));
				Assert.Equal("<existing-iis />", File.ReadAllText(Path.Combine(configDir, "IISUrlRewrite.Staging.config")));
			}
			finally
			{
				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, recursive: true);
			}
		}
	}
}
