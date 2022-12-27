using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private readonly string outBox;
		private readonly string actor;
		private readonly string carbonCopy;
		private readonly string statusActivity;
		private const string ACTIVITYSTREAM_CONTEXT = "https://www.w3.org/ns/activitystreams";
		private const string ACTIVITYSTREAM_PUBLIC = "https://www.w3.org/ns/activitystreams#Public";
		private const string PAGE_TRUE = "?page=true";

		public ActivityPubManager(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);

			var userrelative = string.Format("users/{0}/outbox", dasBlogSettings.SiteConfiguration.MastodonAccount);
			var actorrelative = string.Format("users/{0}", dasBlogSettings.SiteConfiguration.MastodonAccount);
			var ccrelative = string.Format("users/{0}/followers", dasBlogSettings.SiteConfiguration.MastodonAccount);
			outBox = new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root), userrelative).AbsoluteUri;
			actor = new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root), actorrelative).AbsoluteUri;
			carbonCopy = new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root), ccrelative).AbsoluteUri;
			statusActivity = new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root), "users/{0}/statuses/{1}/activity").AbsoluteUri;
		}

		public User GetUser()
		{
			var user = new User
			{
				Context = ACTIVITYSTREAM_CONTEXT,
				Id = outBox,
				Type = "OrderedCollection",
				First = outBox + PAGE_TRUE
			};

			return user;
		}

		public UserPage GetUserPage(IList<Entry> page)
		{
			var ordereditems = page.Select(o => new OrderedItem
				{
					Id = string.Format(statusActivity, dasBlogSettings.SiteConfiguration.MastodonAccount, o.EntryId),
					Type = "Create",
					Actor = actor,
					Published = DateTime.UtcNow,
					To = new List<string>() { ACTIVITYSTREAM_PUBLIC },
					Cc = new List<string>() { carbonCopy },
					Sensitive = false,
					Content = string.Format("New post: {0} {1}", o.Title, o.CompressedTitle)
				}).ToList();

			var userpage = new UserPage
			{
				Id = outBox + PAGE_TRUE,
				Type = "OrderedCollectionPage",
				Next = "",
				Previous = "",
				PartOf = outBox,
				OrderItems = ordereditems
			};

			return userpage;
		}

		public WebFinger WebFinger(string resource)
		{
			// validate resource

			if (string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.MastodonServerUrl) ||
					string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.MastodonAccount))
			{
				return null;
			}

			var usersurl = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl),
						string.Format("users/{0}", dasBlogSettings.SiteConfiguration.MastodonAccount.Remove(0, 1))).AbsoluteUri;

			var accturl = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl),
						string.Format("{0}", dasBlogSettings.SiteConfiguration.MastodonAccount)).AbsoluteUri;

			var authurl = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl),
						"authorize_interaction").AbsoluteUri + "?uri={uri}";

			var webFinger = new WebFinger
			{
				Subject = string.Format("acct:{0}@{1}", dasBlogSettings.SiteConfiguration.MastodonAccount.Remove(0, 1), new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl).Host),
				Aliases = new List<string> { accturl, usersurl },

				Links =  new List<WebFingerLink>
				{
					new WebFingerLink() { Relationship="http://webfinger.net/rel/profile-page", Type="text/html", HRef=accturl },
					new WebFingerLink() { Relationship="self", Type=@"application/activity+json", HRef=usersurl},
					new WebFingerLink() { Relationship="http://ostatus.org/schema/1.0/subscribe", Template=authurl }
				}
			};

			return webFinger;
		}
	}
}
