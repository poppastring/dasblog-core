using DasBlog.Services;
using DasBlog.Services.Atproto;
using Microsoft.Extensions.Configuration;

namespace DasBlog.Web.Services
{
	/// <summary>
	/// Resolves ATProto settings from meta.config with safe defaults.
	/// Follows the same pattern as MastodonSettingsResolver.
	/// </summary>
	public sealed class AtprotoSettingsResolver : IAtprotoSettingsResolver
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IConfiguration configuration;
		private readonly IAtprotoCredentialStore credentialStore;

		public AtprotoSettingsResolver(
			IDasBlogSettings dasBlogSettings,
			IConfiguration configuration,
			IAtprotoCredentialStore credentialStore)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.configuration = configuration;
			this.credentialStore = credentialStore;
		}

		public bool IsEnabled()
		{
			return dasBlogSettings.MetaTags?.AtprotoEnabled ?? false;
		}

		public string GetHandle()
		{
			return dasBlogSettings.MetaTags?.AtprotoHandle ?? string.Empty;
		}

		public string GetPdsUrl()
		{
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.MetaTags?.AtprotoPdsUrl))
			{
				return dasBlogSettings.MetaTags.AtprotoPdsUrl;
			}

			return "https://bsky.social";
		}

		public string GetPublicationRkey()
		{
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.MetaTags?.AtprotoPublicationRkey))
			{
				return dasBlogSettings.MetaTags.AtprotoPublicationRkey;
			}

			return "site";
		}

		public string GetPublicationUri()
		{
			return dasBlogSettings.MetaTags?.AtprotoPublicationUri ?? string.Empty;
		}

		public string GetAppPassword()
		{
			var configuredPassword = configuration["Atproto:AppPassword"];
			if (!string.IsNullOrWhiteSpace(configuredPassword))
			{
				return configuredPassword;
			}

			return credentialStore.GetAppPassword() ?? string.Empty;
		}
	}
}
