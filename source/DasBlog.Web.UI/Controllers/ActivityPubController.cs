using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace DasBlog.Web.Controllers
{
	[Produces("text/json")]
	public class ActivityPubController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActivityPubManager activityPubManager;
		private readonly IBlogManager blogManager;
		private readonly IMapper mapper;
		private static readonly IList<MastodonUserViewModel> followersList = new List<MastodonUserViewModel>();

		public ActivityPubController(IActivityPubManager activityPubManager, IBlogManager blogManager, IDasBlogSettings dasBlogSettings, IMapper mapper) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.activityPubManager = activityPubManager;
			this.blogManager = blogManager;
			this.mapper = mapper;
		}

		[HttpGet(".well-known/webfinger")]
		public ActionResult WebFinger(string resource)
		{
			if (resource.StartsWith("acct:"))
			{
				resource = resource.Remove(0, 5);
			}

			var webfinger = activityPubManager.WebFinger(resource);
			if (webfinger != null)
			{
				var wfvm = mapper.Map<WebFingerViewModel>(webfinger);
				wfvm.links = webfinger.Links.Select(entry => mapper.Map<WebFingerLinkViewModel>(entry)).ToList();

				return Json(wfvm, jsonSerializerOptions);
			}

			return NoContent();
		}

		[HttpGet]
		[Route("@{user}")]
		[Route("users/{user}")]
		public IActionResult Actor(string user)
		{
			var mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			if (string.Compare(user, mastodonAccount, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			var actor = new ActorViewModel
			{
				context = CreateActorContext(),
				id = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}"),
				type = "Person",
				following = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/following"),
				followers = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/followers"),
				inbox = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/inbox"),
				outbox = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/outbox"),
				preferredUsername = mastodonAccount,
				name = dasBlogSettings.SiteConfiguration.Title,
				summary = dasBlogSettings.SiteConfiguration.Description,
				url = dasBlogSettings.RelativeToRoot($"@{mastodonAccount}"),
				published = DateTime.UtcNow.AddDays(-400),
				endpoints = new Endpoints { sharedInbox = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/inbox") },
				publicKey = null,
				icon = null,
				image = null
			};

			return Json(actor, jsonSerializerOptions);
		}

		[HttpGet]
		[Route("users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			string mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			if (string.Compare(user, mastodonAccount, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			if (page)
			{
				var fpentries = blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"]).ToList();

				var userpage = activityPubManager.GetUserPage(fpentries);
				var upvm = mapper.Map<UserPageViewModel>(userpage);

				var context = new List<object>
				{
					"https://www.w3.org/ns/activitystreams",
					GetUserPageContext()
				};

				upvm.context = context.ToArray();
				upvm.orderedItems = userpage.OrderItems.Select(entry => mapper.Map<OrderedItemViewModel>(entry)).ToArray();

				var val = upvm.ToJson();
				return Json(val, jsonSerializerOptions);
			}

			var userinfo = activityPubManager.GetUser();
			var uvm = mapper.Map<UserViewModel>(userinfo);

			return Json(uvm, jsonSerializerOptions);
		}

		[HttpPost]
		[Route("users/{user}/inbox/followers")]
		public IActionResult PostFollow([FromBody] MastodonUserViewModel follower, string user)
		{
			string mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			if (string.Compare(user, mastodonAccount, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			// Verify the content (validate bearer token)

			// Store the follow details, not like this though
			followersList.Add(follower);

			return Ok();
		}

		[HttpGet]
		[Route("users/{user}/inbox/followers")]
		public IActionResult GetFollowers(string user, int? page)
		{
			string mastodonAccount = dasBlogSettings.SiteConfiguration.MastodonAccount;
			if (mastodonAccount.StartsWith("@"))
			{
				mastodonAccount = mastodonAccount.Remove(0, 1);
			}

			if (string.Compare(user, mastodonAccount, StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NotFound();
			}

			if (page == null)
			{
				var ufvm = new UserFollowersViewModel();
				ufvm.context = "https://www.w3.org/ns/activitystreams";
				ufvm.id = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/followers");
				ufvm.first = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/followers?page=1");
				ufvm.type = "OrderedCollection";
				ufvm.totalItems = followersList.Count;

				return Json(ufvm);
			}

			var ufpvm = new UserFollowersPageViewModel();
			ufpvm.context = "https://www.w3.org/ns/activitystreams";
			ufpvm.id = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/followers");
			ufpvm.next = dasBlogSettings.RelativeToRoot($"users/{mastodonAccount}/followers?page=2");
			ufpvm.type = "OrderedCollectionPage";
			ufpvm.totalItems = followersList.Count;
			ufpvm.orderedItems = followersList.Select(follow => follow.userid).ToArray();

			return Json(ufpvm);
		}

		private object[] CreateActorContext()
		{
			var list = new List<object>();
			list.Add("https://www.w3.org/ns/activitystreams");
			list.Add("https://w3id.org/security/v1");

			var ac = new ActorContext
			{
				manuallyApprovesFollowers = "as:manuallyApprovesFollowers",
				toot = "http://joinmastodon.org/ns#",
				featured = new Featured { id = "toot:featured", type = "@id" },
				featuredTags = new Featuredtags { id = "toot:featuredTags", type = "@id" },
				alsoKnownAs = new Alsoknownas { id = "as:alsoKnownAs", type = "@id" },
				movedTo = new Movedto { id = "as:movedTo", type = "@id" },
				schema = "http://schema.org#",
				PropertyValue = "schema:PropertyValue",
				value = "schema:value",
				discoverable = "toot:discoverable",
				Device = "toot:Device",
				Ed25519Signature = "toot:Ed25519Signature",
				Ed25519Key = "toot:Ed25519Key",
				Curve25519Key = "toot:Curve25519Key",
				EncryptedMessage = "toot:EncryptedMessage",
				publicKeyBase64 = "toot:publicKeyBase64",
				deviceId = "toot:deviceId",
				claim = new Claim { id = "toot:Claim", type = "@id" },
				fingerprintKey = new Fingerprintkey { id = "toot:fingerprintKey", type = "@id" },
				identityKey = new Identitykey { id = "toot:identityKey", type = "@id" },
				devices = new Devices { id = "toot:devices", type = "@id" },
				messageFranking = "toot:messageFranking",
				messageType = "toot:messageType",
				cipherText = "toot:cipherText",
				suspended = "toot:suspended",
				Emoji = "toot:Emoji",
				focalPoint = new Focalpoint { id = "toot:focalPoint", container = "@list" }
			};

			list.Add(ac);

			return list.ToArray();
		}

		private static UserPageContextViewModel GetUserPageContext()
		{
			return new UserPageContextViewModel
			{
				ostatus = "http://ostatus.org#",
				atomUri = "ostatus:atomUri",
				inReplyToAtomUri = "ostatus:inReplyToAtomUri",
				conversation = "ostatus:conversation",
				sensitive = "as:sensitive",
				toot = "http://joinmastodon.org/ns#",
				votersCount = "toot:votersCount",
				blurhash = "toot:blurhash",
				focalPoint = new Focalpoint { container = "@list", id = "toot:focalPoint" },
				Hashtag = "as:Hashtag"
			};
		}
	}
}
