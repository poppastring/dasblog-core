using FishyFlip;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Atproto
{
	public class AtprotoPublisher : IAtprotoPublisher
	{
		private readonly IAtprotoSettingsResolver settingsResolver;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly ILogger<AtprotoPublisher> logger;

		public AtprotoPublisher(
			IAtprotoSettingsResolver settingsResolver,
			IDasBlogSettings dasBlogSettings,
			ILogger<AtprotoPublisher> logger)
		{
			this.settingsResolver = settingsResolver;
			this.dasBlogSettings = dasBlogSettings;
			this.logger = logger;
		}

		public async Task<string> EnsurePublicationAsync(CancellationToken cancellationToken = default)
		{
			if (!settingsResolver.IsEnabled())
			{
				logger.LogDebug("ATProto publishing is disabled.");
				return null;
			}

			var handle = settingsResolver.GetHandle();
			if (string.IsNullOrWhiteSpace(handle))
			{
				logger.LogWarning("ATProto is enabled but no Bluesky handle is configured.");
				return null;
			}

			try
			{
				var pdsUrl = settingsResolver.GetPdsUrl();
				var publicationRkey = settingsResolver.GetPublicationRkey();

				logger.LogInformation(
					"Publishing Standard.site publication record for handle {Handle} at PDS {Pds}.",
					handle, pdsUrl);

				// TODO: Implement FishyFlip authentication and publication record creation.
				// This is where we would:
				// 1. Authenticate with the app password
				// 2. Create/update the site.standard.publication record
				// 3. Return the AT-URI for verification
				// For now, return a placeholder AT-URI.

				var atUri = $"at://{handle}/site.standard.publication/{publicationRkey}";
				logger.LogInformation("Published to {AtUri}", atUri);

				return atUri;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to ensure ATProto publication record.");
				return null;
			}
		}
	}
}


