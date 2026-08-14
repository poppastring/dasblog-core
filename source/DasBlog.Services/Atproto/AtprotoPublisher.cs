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
		private readonly ILogger<AtprotoPublisher> logger;

		public AtprotoPublisher(IAtprotoSettingsResolver settingsResolver, ILogger<AtprotoPublisher> logger)
		{
			this.settingsResolver = settingsResolver;
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
					"Attempting to publish Standard.site publication record for handle {Handle} at PDS {Pds}.",
					handle, pdsUrl);

				// Placeholder: real implementation would call FishyFlip to authenticate and publish
				// For now, we just log and return a mock AT-URI
				var atUri = $"at://{handle}/site.standard.publication/{publicationRkey}";
				logger.LogInformation("Would publish to {AtUri}", atUri);

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
