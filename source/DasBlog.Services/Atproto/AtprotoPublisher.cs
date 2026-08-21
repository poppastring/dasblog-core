using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DasBlog.Services.Atproto
{
	public class AtprotoPublisher : IAtprotoPublisher
	{
		private readonly IAtprotoSettingsResolver settingsResolver;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IAtprotoRepositoryClient repositoryClient;
		private readonly ILogger<AtprotoPublisher> logger;

		public AtprotoPublisher(
			IAtprotoSettingsResolver settingsResolver,
			IDasBlogSettings dasBlogSettings,
			IAtprotoRepositoryClient repositoryClient,
			ILogger<AtprotoPublisher> logger)
		{
			this.settingsResolver = settingsResolver;
			this.dasBlogSettings = dasBlogSettings;
			this.repositoryClient = repositoryClient;
			this.logger = logger;
		}

		public string GetPublicationUri()
		{
			return settingsResolver.IsEnabled()
				? settingsResolver.GetPublicationUri()
				: string.Empty;
		}

		public async Task<string> PublishPublicationAsync(CancellationToken cancellationToken = default)
		{
			var session = await CreateSessionAsync(cancellationToken);
			if (session == null)
			{
				return null;
			}

			var publicationRkey = settingsResolver.GetPublicationRkey();
			if (string.IsNullOrWhiteSpace(publicationRkey))
			{
				// Log the misconfiguration and use a safe fallback
				logger.LogWarning("Publication RKey is missing or invalid in settings. Defaulting to 'site'.");
				publicationRkey = "site";
			}
			var publication = new AtprotoPublication
			{
				Url = GetPublicationUrl(),
				Name = dasBlogSettings.SiteConfiguration.Title,
				Description = dasBlogSettings.SiteConfiguration.Description
			};

			await repositoryClient.PutPublicationAsync(session, publicationRkey, publication, cancellationToken);

			var atUri = $"at://{session.Did}/site.standard.publication/{publicationRkey}";
			logger.LogInformation("Published Standard.site publication record to {AtUri}.", atUri);
			return atUri;
		}

		public async Task<bool> DeletePublicationAsync(CancellationToken cancellationToken = default)
		{
			var session = await CreateSessionAsync(cancellationToken);
			if (session == null)
			{
				return false;
			}

			await repositoryClient.DeletePublicationAsync(session, settingsResolver.GetPublicationRkey(), cancellationToken);
			logger.LogInformation("Deleted Standard.site publication record for DID {Did}.", session.Did);
			return true;
		}

		private async Task<AtprotoSession> CreateSessionAsync(CancellationToken cancellationToken)
		{
			if (!settingsResolver.IsEnabled())
			{
				logger.LogWarning("ATProto publishing is disabled.");
				return null;
			}

			var handle = settingsResolver.GetHandle();
			if (string.IsNullOrWhiteSpace(handle))
			{
				logger.LogWarning("ATProto is enabled but no Bluesky handle is configured.");
				return null;
			}

			var appPassword = settingsResolver.GetAppPassword();
			if (string.IsNullOrWhiteSpace(appPassword))
			{
				logger.LogWarning("ATProto is enabled but no app password is configured.");
				return null;
			}

			return await repositoryClient.CreateSessionAsync(
				settingsResolver.GetPdsUrl(),
				handle,
				appPassword,
				cancellationToken);
		}

		private string GetPublicationUrl()
		{
			var baseUrl = dasBlogSettings.GetBaseUrl();
			if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var publicationUri) || publicationUri.Scheme != Uri.UriSchemeHttps)
			{
				throw new InvalidOperationException("ATProto publishing requires the BaseUrl setting (SiteConfiguration.Root) to be an absolute HTTPS URL.");
			}

			return publicationUri.GetLeftPart(UriPartial.Authority).TrimEnd('/');
		}
	}
}


