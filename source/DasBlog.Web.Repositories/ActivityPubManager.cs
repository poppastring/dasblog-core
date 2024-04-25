using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers
{
	public class ActivityPubManager : IActivityPubManager
	{
		private readonly IBlogDataService dataService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly string roothost;
		private readonly string alias;
		private readonly string following;
		private readonly string followers;
		private readonly string inBox;
		private readonly string outBox;
		private const string ACTIVITYSTREAM_CONTEXT = "https://www.w3.org/ns/activitystreams";

		public ActivityPubManager(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);

			roothost = new Uri(dasBlogSettings.SiteConfiguration.Root).AbsoluteUri.Replace("www", "");

			var followingrelative = string.Format("users/{0}/following", dasBlogSettings.SiteConfiguration.MastodonAccount);
			var followersrelative = string.Format("users/{0}/followers", dasBlogSettings.SiteConfiguration.MastodonAccount);
			
			following = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl), followingrelative).AbsoluteUri;
			followers = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl), followersrelative).AbsoluteUri;
			inBox = new Uri(new Uri(roothost), "api/inbox").AbsoluteUri;
			outBox = new Uri(new Uri(roothost), "api/outbox").AbsoluteUri;
			alias = new Uri(new Uri(roothost), "@blog").AbsoluteUri;
		}

		public WebFinger WebFinger()
		{
			var webFinger = new WebFinger
			{
				subject = $"acct:blog@{roothost}",
				aliases = [alias],

				links = [
					new Link() { rel="self", type=@"application/activity+json", href= alias },
					new Link() { rel="http://webfinger.net/rel/profile-page", type="text/html", href=roothost }
				]
			};

			return webFinger;
		}

		public Actor Actor()
		{
			var actor = new Actor()
			{
				context = ACTIVITYSTREAM_CONTEXT,
				id = alias,
				type = "Person",
				following = following,
				followers = followers,
				inbox = inBox,
				outbox = outBox,
				preferredUsername = "blog",
				name = dasBlogSettings.SiteConfiguration.Title,
				summary = dasBlogSettings.SiteConfiguration.Description,
				url = dasBlogSettings.SiteConfiguration.Root,
				discoverable = true,
				memorial = false,
				icon = new() {  url = dasBlogSettings.SiteConfiguration.ChannelImageUrl },
				image = new() { url = dasBlogSettings.SiteConfiguration.ChannelImageUrl },
				publicKey = new() { id = alias + "#main-key", owner = alias, publicKeyPem = "" },
				attachment =
				[
					new() { name="Blog", type ="PropertyValue", value = dasBlogSettings.SiteConfiguration.Root },
					new() { name="RSS", type ="PropertyValue", value = dasBlogSettings.RssUrl }
				]
			};


			return actor;
		}
	}
}
