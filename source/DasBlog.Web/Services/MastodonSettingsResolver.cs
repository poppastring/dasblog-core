using DasBlog.Services;

namespace DasBlog.Web.Services
{
	/// <summary>
	/// Resolves Mastodon settings while we migrate canonical storage from site.config to meta.config.
	/// Meta config wins; site config remains the fallback so existing installs continue to work.
	/// </summary>
	public sealed class MastodonSettingsResolver : IMastodonSettingsResolver
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public MastodonSettingsResolver(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public string GetMastodonServerUrl()
		{
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.MetaTags?.MastodonServerUrl))
			{
				return dasBlogSettings.MetaTags.MastodonServerUrl;
			}

			return dasBlogSettings.SiteConfiguration.MastodonServerUrl;
		}

		public string GetMastodonAccount()
		{
			if (!string.IsNullOrWhiteSpace(dasBlogSettings.MetaTags?.MastodonAccount))
			{
				return dasBlogSettings.MetaTags.MastodonAccount;
			}

			return dasBlogSettings.SiteConfiguration.MastodonAccount;
		}
	}
}
