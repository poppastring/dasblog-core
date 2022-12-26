using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using newtelligence.DasBlog.Runtime;
using Quartz.Util;

namespace DasBlog.Managers
{
	public class ActivityPubManager : IActivityPubManager
	{
		private readonly IBlogDataService dataService;
		private readonly IDasBlogSettings dasBlogSettings;

		public ActivityPubManager(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;

			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);
		}

		public User GetUser()
		{
			throw new NotImplementedException();
		}

		public UserPage GetUserPage()
		{
			throw new NotImplementedException();
		}

		public WebFinger WebFinger(string resource)
		{
			// validate resource

			if (dasBlogSettings.SiteConfiguration.MastodonServerUrl.IsNullOrWhiteSpace() ||
				dasBlogSettings.SiteConfiguration.MastodonAccount.IsNullOrWhiteSpace())
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
