using DasBlog.Services.ConfigFile;

namespace DasBlog.Services.FileManagement.Interfaces
{
	public interface ISiteSecurityConfigFileService
	{
		SiteSecurityConfigData LoadConfig();
	}
}
