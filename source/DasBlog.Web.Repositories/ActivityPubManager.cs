using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityPub;
using DasBlog.Services.ActivityPub.Helper;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers
{
	public class ActivityPubManager : IActivityPubManager
	{
		private readonly IBlogDataService dataService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActorService actorService;
		private readonly string roothost, alias, following, followers, inBox, outBox, notes, replies, users, icon;
		private readonly string tags, authorUsername, authorUrl, authorUserid, rootdomain, template;
		private readonly List<string> followersList = new List<string>();

		private const string ACTIVITYSTREAM_CONTEXT = "https://www.w3.org/ns/activitystreams";

		public ActivityPubManager(IDasBlogSettings settings, IActorService actorservice)
		{
			dasBlogSettings = settings;
			actorService = actorservice;
			var loggingDataService = LoggingDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.LogDir));
			dataService = BlogDataServiceFactory.GetService(Path.Combine(dasBlogSettings.WebRootDirectory, dasBlogSettings.SiteConfiguration.ContentDir), loggingDataService);

			roothost = new Uri(dasBlogSettings.SiteConfiguration.Root).AbsoluteUri.Replace("www", "");
			rootdomain = new Uri(dasBlogSettings.SiteConfiguration.Root).Host.Replace("www", "");
			var authdomain = new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl).Host.Replace("www", "");

			var followingrelative = string.Format("users/{0}/following", dasBlogSettings.SiteConfiguration.MastodonAccount);
			var followersrelative = string.Format("users/{0}/followers", dasBlogSettings.SiteConfiguration.MastodonAccount);
			
			following = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl), followingrelative).AbsoluteUri;
			followers = new Uri(new Uri(dasBlogSettings.SiteConfiguration.MastodonServerUrl), followersrelative).AbsoluteUri;
			inBox = new Uri(new Uri(roothost), "api/inbox").AbsoluteUri;
			outBox = new Uri(new Uri(roothost), "api/outbox").AbsoluteUri;
			alias = new Uri(new Uri(roothost), "@blog").AbsoluteUri;
			notes = new Uri(new Uri(roothost), "api/notes").AbsoluteUri;
			replies = new Uri(new Uri(roothost), "api/replies").AbsoluteUri;
			tags = new Uri(new Uri(roothost), "api/tags").AbsoluteUri;
			users = new Uri(new Uri(roothost), "api/users/blog").AbsoluteUri;
			template = new Uri(new Uri(roothost), "authorize_interaction?uri=").AbsoluteUri;
			icon = new Uri(new Uri(dasBlogSettings.SiteConfiguration.Root), dasBlogSettings.SiteConfiguration.ChannelImageUrl).AbsoluteUri;

			authorUsername = $"@{dasBlogSettings.SiteConfiguration.MastodonAccount}@{authdomain}";
			authorUrl = new Uri(new Uri(roothost), $"users/{dasBlogSettings.SiteConfiguration.MastodonAccount}").AbsoluteUri;
			authorUserid = dasBlogSettings.SiteConfiguration.MastodonAccount;
		}

		public WebFinger GetWebFinger()
		{
			var webFinger = new WebFinger
			{
				subject = $"acct:blog@{rootdomain}",
				aliases = [alias, users],

				links = [
					new Link() { rel="self", type=@"application/activity+json", href=alias },
					new Link() { rel="http://webfinger.net/rel/profile-page", type="text/html", href=dasBlogSettings.SiteConfiguration.Root },
					new Link() { rel="http://ostatus.org/schema/1.0/subscribe", template=template + "{uri}" },
					new Link() { rel="http://webfinger.net/rel/avatar", type="image/jpeg", href=icon }
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
				icon = new() {  url = icon, mediaType = "image/jpeg" },
				image = new() { url = icon, mediaType = "image/jpeg" },
				publicKey = new() { id = alias + "#main-key", owner = alias, publicKeyPem = dasBlogSettings.SiteConfiguration.MastodonPublicKey },
				attachment =
				[
					new() { name="Blog", type ="PropertyValue", value = GetAttachment(dasBlogSettings.SiteConfiguration.Root,  dasBlogSettings.SiteConfiguration.Root) },
					new() { name="RSS", type ="PropertyValue", value = GetAttachment(dasBlogSettings.SiteConfiguration.Root, dasBlogSettings.RssUrl) }
				]
			};

			return actor;
		}

		public Outbox GenerateOutbox(EntryCollection entries)
		{
			var items = entries
				.Select(item => new
				{
					Title = item.Title,
					Link = dasBlogSettings.RelativeToRoot((dasBlogSettings.GeneratePostUrl(item))),
					Description = dasBlogSettings.SiteConfiguration.Description,
					PubDate = item.CreatedUtc,
					Hash = item.EntryId,
					Tags = item.Categories.Split(',').ToList().Select(x => dasBlogSettings.CompressTitle(x)).ToList()
				});

			var ordereditems = new List<Ordereditem>();

			foreach (var item in items)
			{
				var note = GetNote(item);
				var createNote = GetCreateNote(note);

				ordereditems.Add(createNote);
			}

			return GetOutbox(ordereditems.ToArray());
		}

		public async Task Follow(InboxMessage message)
		{
			var target = message.Object!.ToString();

			// get actor info
			var actor = await ActorService.FetchActorInformationAsync(message.Actor);

			// temporary follower to a persistent list
			followersList.Add(actor.id);

			var acceptRequest = new AcceptRequest()
			{
				context = "https://www.w3.org/ns/activitystreams",
				id = $"{target}#accepts/follows/{actor.id}",
				actor = $"{target}",
				Object = new
				{
					message.Id,
					Actor = actor.url,
					Type = "Follow",
					Object = $"{target}"
				}
			};

			await actorService.SendSignedRequest(
							JsonSerializer.Serialize(acceptRequest, ActorService.SerializerOptions),
							new Uri(actor.inbox));

		}

		public async Task Unfollow(InboxMessage message, string requestbody)
		{
			// delete follower from persistent list

			// get actor info
			var actor = await ActorService.FetchActorInformationAsync(message.Actor);

			var uuid = Guid.NewGuid().ToString();

			var acceptRequest = new AcceptRequest()
			{
				context = "https://www.w3.org/ns/activitystreams",
				id = new Uri(new Uri(roothost), uuid).AbsoluteUri,
				actor = alias,
				Object = JsonSerializer.Deserialize<dynamic>(requestbody, ActorService.SerializerOptions)!
			};

			// Send signed request to the actor's inbox of the unfollow
			await actorService.SendSignedRequest(
								JsonSerializer.Serialize(acceptRequest, ActorService.SerializerOptions), 
								new Uri(actor.inbox));
		}

		public async Task Like(InboxMessage message)
		{
			var actor = await ActorService.FetchActorInformationAsync(message.Actor);


		}

		public async Task AddReply(InboxMessage message)
		{ 
			// add reply

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

		private Note GetNote(dynamic item)
		{
			var tags = new List<Tag>()
			{
				new Tag() { Type = "Mention", Href = authorUrl, Name = authorUsername }
			};

			var itemTags = item?.Tags as List<string> ?? [];

			if (itemTags?.Count > 0)
			{
				foreach (var tag in item?.Tags ?? Enumerable.Empty<string>())
				{
					tags.Add(new Tag()
					{
						Type = "Hashtag",
						Href = $"{this.tags}/{tag}",
						Name = $"#{tag}"
					});
				}
			}

			var noteId = $"{notes}/{item.Hash}";

			var note = new Note()
			{
				context = "https://www.w3.org/ns/activitystreams",
				id = noteId,
				type = "Note",
				hash = item.Hash,
				content = GetContent(item!, this.tags, authorUserid, authorUrl),
				url = item!.Link!,
				attributedTo = alias, // domain/@blog
				to = new List<string>() { "https://www.w3.org/ns/activitystreams#Public" }.ToArray(),
				cc = new List<string>().ToArray(),
				published = item.PubDate.ToString("yyyy-MM-ddTHH:mm:sszzz"),
				tag = tags.ToArray(),
				replies = new Replies
				{
					id = $"{replies}/{item.Hash}",
					type = "Collection",
					first = new First
					{
						type = "CollectionPage",
						next = $"{replies}/{item.Hash}?page=true",
						partOf = $"{replies}/{item.Hash}",
						items = new List<string>().ToArray()
					}
				}
			};

			return note;
		}

		private Ordereditem GetCreateNote(Note note)
		{
			var createNote = new Ordereditem
			{
				context = "https://www.w3.org/ns/activitystreams",
				id = $"{note.id}/create",
				type = "Create",
				actor = alias,
				to = new List<string>() { "https://www.w3.org/ns/activitystreams#Public" }.ToArray(),
				cc = new List<string>().ToArray(),
				published = note.published,
				_object = note
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

		private static string GetAttachment(string tag, string link)
		{
			return $"<a href=\"{link}\" target=\"_blank\" rel=\"nofollow noopener noreferrer me\" translate=\"no\"><span class=\"invisible\">https://</span><span class=\"\">{tag}</span><span class=\"invisible\"></span></a>";
		}	
	}
}
