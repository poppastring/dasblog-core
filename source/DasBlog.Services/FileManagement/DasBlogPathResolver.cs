using System.IO;
using Microsoft.Extensions.Configuration;

namespace DasBlog.Services.FileManagement
{
	public class DasBlogPathResolver : IDasBlogPathResolver
	{
		private const string BinariesUrlRelative = "content/binary";

		public string ContentRootPath { get; }

		public string SiteConfigFilePath { get; }
		public string MetaConfigFilePath { get; }
		public string SecurityConfigFilePath { get; }
		public string IISUrlRewriteFilePath { get; }
		public string OEmbedProvidersFilePath { get; }

		public string DefaultSiteConfigRelativePath { get; }
		public string SiteConfigRelativePath { get; }
		public string DefaultMetaConfigRelativePath { get; }
		public string MetaConfigRelativePath { get; }
		public string IISUrlRewriteRelativePath { get; }
		public string DefaultIISUrlRewriteRelativePath { get; }
		public string DefaultSecurityConfigRelativePath { get; }
		public string SecurityConfigRelativePath { get; }
		public string OEmbedProvidersRelativePath { get; }

		public string ThemeFolderPath { get; }
		public string BinariesPath { get; }
		public string BinariesUrlRelativePath => BinariesUrlRelative;
		public string LogFolderPath { get; }
		public string ContentFolderPath { get; }
		public string RadioStoriesFolderPath { get; }

		public DasBlogPathResolver(string contentRootPath, string environmentName, IConfiguration configuration)
		{
			ContentRootPath = contentRootPath;

			DefaultSiteConfigRelativePath = Path.Combine("Config", "site.config");
			SiteConfigRelativePath = Path.Combine("Config", $"site.{environmentName}.config");
			DefaultMetaConfigRelativePath = Path.Combine("Config", "meta.config");
			MetaConfigRelativePath = Path.Combine("Config", $"meta.{environmentName}.config");
			DefaultSecurityConfigRelativePath = Path.Combine("Config", "siteSecurity.config");
			SecurityConfigRelativePath = Path.Combine("Config", $"siteSecurity.{environmentName}.config");
			DefaultIISUrlRewriteRelativePath = Path.Combine("Config", "IISUrlRewrite.config");
			IISUrlRewriteRelativePath = Path.Combine("Config", $"IISUrlRewrite.{environmentName}.config");
			OEmbedProvidersRelativePath = Path.Combine("Config", "oembed-providers.json");

			SiteConfigFilePath = Path.Combine(contentRootPath, SiteConfigRelativePath);
			MetaConfigFilePath = Path.Combine(contentRootPath, MetaConfigRelativePath);
			SecurityConfigFilePath = Path.Combine(contentRootPath, SecurityConfigRelativePath);
			IISUrlRewriteFilePath = Path.Combine(contentRootPath, IISUrlRewriteRelativePath);
			OEmbedProvidersFilePath = Path.Combine(contentRootPath, OEmbedProvidersRelativePath);

			var theme = configuration.GetSection("Theme").Value ?? "dasblog";
			ThemeFolderPath = Path.GetFullPath(Path.Combine(contentRootPath, "Themes", theme));

			var binariesDir = configuration.GetValue<string>("BinariesDir") ?? Path.Combine("content", "binary");
			BinariesPath = Path.GetFullPath(Path.Combine(contentRootPath, binariesDir));

			var logDirFromConfig = configuration.GetSection("LogDir").Value ?? "logs";
			LogFolderPath = Path.IsPathRooted(logDirFromConfig)
				? logDirFromConfig
				: Path.Combine(contentRootPath, logDirFromConfig);

			var contentDir = configuration.GetValue<string>("ContentDir") ?? "content";
			ContentFolderPath = Path.GetFullPath(Path.Combine(contentRootPath, contentDir));

			RadioStoriesFolderPath = Path.GetFullPath(Path.Combine(contentRootPath, "content", "radioStories"));
		}

		public static void EnsureDefaultConfigFiles(string contentRootPath, string environmentName)
		{
			var securityEnvPath = Path.Combine(contentRootPath, "Config", $"siteSecurity.{environmentName}.config");
			var securityDefaultPath = Path.Combine(contentRootPath, "Config", "siteSecurity.config");
			if (!File.Exists(securityEnvPath) && File.Exists(securityDefaultPath))
			{
				File.Copy(securityDefaultPath, securityEnvPath);
			}

			var iisEnvPath = Path.Combine(contentRootPath, "Config", $"IISUrlRewrite.{environmentName}.config");
			var iisDefaultPath = Path.Combine(contentRootPath, "Config", "IISUrlRewrite.config");
			if (!File.Exists(iisEnvPath) && File.Exists(iisDefaultPath))
			{
				File.Copy(iisDefaultPath, iisEnvPath);
			}
		}
	}
}
