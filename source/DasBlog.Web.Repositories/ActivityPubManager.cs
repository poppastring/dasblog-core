using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Services.Rss.Rss20;
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
		private readonly string notes;
		private readonly string replies;
		private readonly string tags;
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
			notes = new Uri(new Uri(roothost), "notes").AbsoluteUri;
			replies = new Uri(new Uri(roothost), "replies").AbsoluteUri;
			tags = new Uri(new Uri(roothost), "tags").AbsoluteUri;
		}

		public WebFinger GetWebFinger()
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

		public Actor GetActor()
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

		public Outbox GenerateOutbox(EntryCollection entries)
		{
			foreach(var item in entries)
			{
				
			}

			throw new NotImplementedException();
		}

		private Outbox GetOutbox(Ordereditem [] orderedItems)
		{
			// Create outbox JSON structure
			var outbox = new Outbox
			{
				context = "https://www.w3.org/ns/activitystreams",
				id = outBox,
				type = "OrderedCollection",
				summary = dasBlogSettings.SiteConfiguration.Description,
				totalItems = orderedItems.Count(),
				orderedItems = orderedItems
			};

			return outbox;
		}

		private dynamic GetNote(dynamic item, string AuthorUsername, string AuthorUrl, string AuthorUserId)
		{
			var itemHash = GetLinkUniqueHash(item.Link!);

			var tags = new List<Tag>()
			{
				new Tag() { Type = "Mention", Href = AuthorUrl, Name = AuthorUsername }
			};

			var itemTags = item?.Tags as List<string> ?? [];

			if (itemTags?.Count > 0)
			{
				foreach (var tag in item?.Tags ?? Enumerable.Empty<string>())
				{
					tags.Add(new Tag()
					{
						Type = "Hashtag",
						Href = $"{tags}/{tag}",
						Name = $"#{tag}"
					});
				}
			}

			var noteId = $"{notes}/{itemHash}";

			var note = new
			{
				_context = "https://www.w3.org/ns/activitystreams",
				id = noteId,
				type = "Note",
				hash = itemHash,
				content = GetContent(item!, this.tags, AuthorUserId, AuthorUrl),
				url = item!.Link!,
				attributedTo = alias, // domain/@blog
				to = new List<string>() { "https://www.w3.org/ns/activitystreams#Public" },
				cc = new List<string>(),
				published = ParsePubDate(item.PubDate),
				tag = tags,
				replies = new
				{
					id = $"{replies}/{itemHash}",
					type = "Collection",
					first = new
					{
						type = "CollectionPage",
						next = $"{replies}/{itemHash}?page=true",
						partOf = $"{replies}/{itemHash}",
						items = new List<string>()
					}
				}
			};

			return note;
		}

		private string? ParsePubDate(string? pubDate)
		{
			if (DateTimeOffset.TryParse(pubDate, out DateTimeOffset parsedDate))
			{
				return parsedDate.ToString("yyyy-MM-ddTHH:mm:sszzz");
			}

			// If parsing fails, return the original string
			return pubDate;
		}

		private dynamic GetCreateNote(dynamic note, string siteactoruri)
		{
			var createNote = new
			{
				_context = "https://www.w3.org/ns/activitystreams",
				id = $"{note.id}/create",
				type = "Create",
				actor = siteactoruri,
				to = new List<string>() { "https://www.w3.org/ns/activitystreams#Public" },
				cc = new List<string>(),
				published = note.published,
				@object = note
			};

			return createNote;
		}

		private string GetContent(dynamic item, string baseTagUrl, string authorName, string authorUri)
		{
			var contentTemplate = "<p>{0}</p><p>{1}</p><p>Full article by {2}: <a href='{3}'>{3}</a></p><p>{4}</p>";

			var tags = string.Empty;

			var itemTags = item?.Tags as List<string> ?? [];

			if (itemTags?.Count > 0)
			{
				foreach (var tag in item?.Tags ?? Enumerable.Empty<string>())
				{
					tags += $" {GetHashTag(tag, $"{baseTagUrl}/{tag}")}";
				}
			}

			var content = string.Format(contentTemplate,
				item!.Title!,
				item!.Description!,
				GetMention(authorName, authorUri),
				item!.Link!,
				tags);

			return content;
		}

		private static string GetHashTag(string tag, string link)
		{
			return $"<a href =\"{link}\" class=\"mention hashtag\" rel=\"tag\">#<span>{tag}</span></a>";
		}

		private static string GetMention(string name, string link)
		{
			return $"<a href=\"{link}\" class=\"u-url mention\">@<span>{name}</span></a>";
		}

		private static string GetLinkUniqueHash(string input)
		{
			using (MD5 md5 = MD5.Create())
			{
				// Convert the input string to a byte array and compute the hash.
				byte[] inputBytes = Encoding.UTF8.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to a hexadecimal string representation.
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("x2")); // "x2" means hexadecimal with two digits.
				}

				return sb.ToString();
			}
		}
	}
}
