using System;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json;
using DasBlog.Services;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Quartz.Util;

namespace DasBlog.Web.Controllers
{
	[Route(".well-known")]
	public class ActivityPubController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public ActivityPubController(IDasBlogSettings settings) : base(settings) 
		{
			dasBlogSettings = settings;
		}

		[Produces("text/json")]
		[HttpGet("webfinger")]
		public ActionResult WebFinger(string resource)
		{
			string mastodonUrl = dasBlogSettings.SiteConfiguration.MastodonServerUrl;
			string mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (string.IsNullOrEmpty(mastodonUrl) || string.IsNullOrEmpty(mastodonAccount))
			{
				return NotFound();
			}
			if ( !mastodonUrl.Contains("://"))
			{
				mastodonUrl = "https://" + mastodonUrl;
			}
			if ( mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			var mastotonSiteUri = new Uri(mastodonUrl);
			string usersUrl = new Uri(mastotonSiteUri, $"users/{mastodonAccount}").AbsoluteUri;
			string accountUrl = new Uri(mastotonSiteUri,	mastodonAccount).AbsoluteUri;
			string authurl = new Uri(mastotonSiteUri, "authorize_interaction").AbsoluteUri + "?uri={uri}";

			if (dasBlogSettings.SiteConfiguration.MastodonServerUrl.IsNullOrWhiteSpace() || 
				dasBlogSettings.SiteConfiguration.MastodonAccount.IsNullOrWhiteSpace())
			{
				return NoContent();
			}

			var results = new Root
			{
				subject = $"acct:{mastodonAccount}@{mastotonSiteUri.Host}",
				aliases = new List<string> { accountUrl, usersUrl },

				links = new List<Link>
				{
					new Link() { rel="http://webfinger.net/rel/profile-page", type="text/html", href= accountUrl },
					new Link() { rel="self", type=@"application/activity+json", href= usersUrl},
					new Link() { rel="http://ostatus.org/schema/1.0/subscribe", template= authurl }
				}
			};

			var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

			return Json(results, options);
		}
	}
}
