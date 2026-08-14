using DasBlog.Services;
using DasBlog.Services.Atproto;
using Microsoft.Extensions.Options;

namespace DasBlog.Web.Services
{
	/// <summary>
	/// Resolves ATProto settings from meta.config with safe defaults.
	/// Follows the same pattern as MastodonSettingsResolver.
	/// </summary>
	public sealed class AtprotoSettingsResolver : IAtprotoSettingsResolver
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public AtprotoSettingsResolver(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
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
	}
}
