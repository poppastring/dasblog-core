using DasBlog.Services.Atproto;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace DasBlog.Web.Services
{
	public sealed class AtprotoCredentialStore : IAtprotoCredentialStore
	{
		private const string ProtectorPurpose = "DasBlog.Atproto.AppPassword.v1";
		private readonly ISiteSecurityConfigFileService configFileService;
		private readonly IConfigFileService<SiteSecurityConfigData> saveConfigFileService;
		private readonly IDataProtector protector;

		public AtprotoCredentialStore(
			ISiteSecurityConfigFileService configFileService,
			IConfigFileService<SiteSecurityConfigData> saveConfigFileService,
			IDataProtectionProvider dataProtectionProvider)
		{
			this.configFileService = configFileService;
			this.saveConfigFileService = saveConfigFileService;
			protector = dataProtectionProvider.CreateProtector(ProtectorPurpose);
		}

		public string GetAppPassword()
		{
			var config = configFileService.LoadConfig();
			return string.IsNullOrWhiteSpace(config.AtprotoAppPassword)
				? string.Empty
				: protector.Unprotect(config.AtprotoAppPassword);
		}

		public void SaveAppPassword(string appPassword)
		{
			var config = configFileService.LoadConfig();
			config.AtprotoAppPassword = string.IsNullOrWhiteSpace(appPassword)
				? string.Empty
				: protector.Protect(appPassword);

			saveConfigFileService.SaveConfig(config);
		}

	}
}
