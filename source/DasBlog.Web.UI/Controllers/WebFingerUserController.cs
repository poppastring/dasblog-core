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
	public class WebFingerUserController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public WebFingerUserController(IDasBlogSettings settings) : base(settings) 
		{
			dasBlogSettings = settings;
		}

		[Produces("text/json")]
		[HttpGet("/.well-known/webfinger")]
		public ActionResult WebFinger(string resource)
		{
			string usersurl = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl),
						string.Format("users/{0}", dasBlogSettings.SiteConfiguration.MastodonAccount.Remove(0,1))).AbsoluteUri;

			string accturl = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl),
						string.Format("{0}", dasBlogSettings.SiteConfiguration.MastodonAccount)).AbsoluteUri;

			string authurl = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl), 
						"authorize_interaction").AbsoluteUri + "?uri={uri}";

			if (dasBlogSettings.SiteConfiguration.MastodonServerUrl.IsNullOrWhiteSpace() || 
				dasBlogSettings.SiteConfiguration.MastodonAccount.IsNullOrWhiteSpace())
			{
				return NoContent();
			}

			var results = new Root
			{
				subject = string.Format("acct:{0}@{1}", dasBlogSettings.SiteConfiguration.MastodonAccount.Remove(0, 1), new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl).Host),
				aliases = new List<string> { accturl, usersurl },

				links = new List<Link>
				{
					new Link() { rel="http://webfinger.net/rel/profile-page", type="text/html", href=accturl },
					new Link() { rel="self", type=@"application/activity+json", href=usersurl},
					new Link() { rel="http://ostatus.org/schema/1.0/subscribe", template=authurl }
				}
			};

			var options = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

			return Json(results, options);
		}
	}
}
