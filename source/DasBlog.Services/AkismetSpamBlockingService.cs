using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.Services.ConfigFile.Interfaces;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using Subtext.Akismet;
using AkismetComment = Subtext.Akismet.Comment;

namespace DasBlog.Services
{
	/// <summary>
	/// <see cref="ISpamBlockingService"/> implementation backed by the
	/// <see href="https://akismet.com/">Akismet</see> service.
	/// </summary>
	public class AkismetSpamBlockingService : ISpamBlockingService
	{
		private readonly ISiteConfig siteConfig;
		private readonly ILogger<AkismetSpamBlockingService> logger;

		public AkismetSpamBlockingService(ISiteConfig siteConfig, ILogger<AkismetSpamBlockingService> logger)
		{
			this.siteConfig = siteConfig;
			this.logger = logger;
		}

		public bool IsEnabled =>
			siteConfig.EnableSpamModeration && !string.IsNullOrWhiteSpace(siteConfig.AkismetAPIKey);

		public async Task<bool> IsSpamAsync(IFeedback feedback, CancellationToken cancellationToken = default)
		{
			if (!IsEnabled || feedback == null)
			{
				return false;
			}

			try
			{
				var client = CreateClient();
				return await client.CheckCommentForSpamAsync(BuildComment(feedback), cancellationToken).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Akismet spam check failed; allowing comment through.");
				return false;
			}
		}

		public void ReportSpam(IFeedback feedback)
		{
			if (!IsEnabled || feedback == null)
			{
				return;
			}

			// Fire-and-forget: the admin doesn't need to wait for Akismet to acknowledge.
			_ = ReportSpamCoreAsync(feedback);
		}

		public void ReportNotSpam(IFeedback feedback)
		{
			if (!IsEnabled || feedback == null)
			{
				return;
			}

			_ = ReportNotSpamCoreAsync(feedback);
		}

		private async Task ReportSpamCoreAsync(IFeedback feedback)
		{
			try
			{
				await CreateClient().SubmitSpamAsync(BuildComment(feedback)).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Akismet SubmitSpam failed (fire-and-forget).");
			}
		}

		private async Task ReportNotSpamCoreAsync(IFeedback feedback)
		{
			try
			{
				await CreateClient().SubmitHamAsync(BuildComment(feedback)).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogWarning(ex, "Akismet SubmitHam failed (fire-and-forget).");
			}
		}

		private AkismetClient CreateClient()
		{
			return new AkismetClient(siteConfig.AkismetAPIKey, new Uri(siteConfig.Root));
		}

		private static AkismetComment BuildComment(IFeedback feedback)
		{
			IPAddress.TryParse(feedback.AuthorIPAddress, out var ip);

			return new AkismetComment(ip ?? IPAddress.None, feedback.AuthorUserAgent ?? string.Empty)
			{
				Author = feedback.Author,
				AuthorEmail = feedback.AuthorEmail,
				AuthorUrl = TryParseUri(feedback.AuthorHomepage),
				Content = feedback.Content,
				Referer = feedback.Referer,
				CommentType = feedback.FeedbackType
			};
		}

		private static Uri TryParseUri(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}

			return Uri.TryCreate(value, UriKind.Absolute, out var uri) ? uri : null;
		}
	}
}
