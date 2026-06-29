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
		private readonly IMastodonSettingsResolver mastodonSettingsResolver;
		private readonly string outBox;
		private readonly string actor;
		private readonly string carbonCopy;
		private readonly string statusActivity;
		private const string ACTIVITYSTREAM_CONTEXT = "https://www.w3.org/ns/activitystreams";
		private const string ACTIVITYSTREAM_PUBLIC = "https://www.w3.org/ns/activitystreams#Public";
		private const string PAGE_TRUE = "?page=true";

		public ActivityPubManager(IDasBlogSettings settings, IBlogDataService dataService, IMastodonSettingsResolver mastodonSettingsResolver)
		{
			dasBlogSettings = settings;
			this.dataService = dataService;
			this.mastodonSettingsResolver = mastodonSettingsResolver;

			var mastodonAccount = this.mastodonSettingsResolver.GetMastodonAccount();
			var userrelative = string.Format("users/{0}/outbox", mastodonAccount);
			var actorrelative = string.Format("users/{0}", mastodonAccount);
			var ccrelative = string.Format("users/{0}/followers", mastodonAccount);
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
			var mastodonAccount = mastodonSettingsResolver.GetMastodonAccount();
			var ordereditems = page.Select(o => new OrderedItem
				{
					Id = string.Format(statusActivity, mastodonAccount, o.EntryId),
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
			string mastodonUrl = mastodonSettingsResolver.GetMastodonServerUrl();
			string mastodonAccount = mastodonSettingsResolver.GetMastodonAccount();

			if (string.IsNullOrWhiteSpace(resource) || string.IsNullOrWhiteSpace(mastodonUrl) || string.IsNullOrWhiteSpace(mastodonAccount))
			{
				return null;
			}

			if (resource.StartsWith("@", StringComparison.Ordinal))
			{
				resource = resource.Remove(0, 1);
			}

			if (mastodonAccount.StartsWith("@", StringComparison.Ordinal))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}
		
			if (!mastodonUrl.Contains("://"))
			{
				mastodonUrl = "https://" + mastodonUrl;
			}

			var mastotonSiteUri = new Uri(mastodonUrl);
			string usersUrl = new Uri(mastotonSiteUri, $"users/{mastodonAccount}").AbsoluteUri;
			string accountUrl = new Uri(mastotonSiteUri, $"@{mastodonAccount}").AbsoluteUri;
			string authurl = new Uri(mastotonSiteUri, "authorize_interaction").AbsoluteUri + "?uri={uri}";

			if (string.IsNullOrWhiteSpace(mastodonUrl) ||
				string.IsNullOrWhiteSpace(mastodonAccount))
			{
				return null;
			}

			var webFinger = new WebFinger
			{
				Subject = $"acct:{mastodonAccount}@{mastotonSiteUri.Host}",
				Aliases = new List<string> { accountUrl, usersUrl },

				Links = new List<WebFingerLink>
				{
					new WebFingerLink() { Relationship="http://webfinger.net/rel/profile-page", Type="text/html", HRef= accountUrl },
					new WebFingerLink() { Relationship="self", Type=@"application/activity+json", HRef= usersUrl},
					new WebFingerLink() { Relationship="http://ostatus.org/schema/1.0/subscribe", Template= authurl }
				}
			};

			return webFinger;
		}
	}
}
