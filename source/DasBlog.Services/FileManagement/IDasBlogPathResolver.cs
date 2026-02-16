namespace DasBlog.Services.FileManagement
{
	public interface IDasBlogPathResolver
	{
		string ContentRootPath { get; }

		string SiteConfigFilePath { get; }
		string MetaConfigFilePath { get; }
		string SecurityConfigFilePath { get; }
		string IISUrlRewriteFilePath { get; }
		string OEmbedProvidersFilePath { get; }

		string DefaultSiteConfigRelativePath { get; }
		string SiteConfigRelativePath { get; }
		string DefaultMetaConfigRelativePath { get; }
		string MetaConfigRelativePath { get; }
		string IISUrlRewriteRelativePath { get; }
		string DefaultIISUrlRewriteRelativePath { get; }
		string DefaultSecurityConfigRelativePath { get; }
		string SecurityConfigRelativePath { get; }
		string OEmbedProvidersRelativePath { get; }

		string ThemeFolderPath { get; }
		string BinariesPath { get; }
		string BinariesUrlRelativePath { get; }
		string LogFolderPath { get; }
		string ContentFolderPath { get; }
		string RadioStoriesFolderPath { get; }
	}
}
