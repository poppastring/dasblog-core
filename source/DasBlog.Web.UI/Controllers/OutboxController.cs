using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	/// <summary>
	/// How clients can post Activities to this actor's outbox 
	/// A feed of whatever the user shares publicly.
	/// </summary>
	[Produces("text/json")]
	public class OutboxController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActivityPubManager activityPubManager;

		public OutboxController(IActivityPubManager pubManager, IDasBlogSettings settings) : base(settings)
		{
			dasBlogSettings = settings;
			activityPubManager = pubManager;
		}

		[HttpGet]
		[Route("/users/{user}/outbox")]
		public IActionResult GetUser(string user, bool page)
		{
			if(string.Compare(user, dasBlogSettings.SiteConfiguration.MastodonAccount, System.StringComparison.InvariantCultureIgnoreCase) != 0)
			{
				return NoContent();
			}

			if(page)
			{
				//{
				//	"id": "https://poppastring.com/users/mdownie/outbox?page=true",
				//	"type": "OrderedCollectionPage",
				//	"next": "https://poppastring.com/users/mdownie/outbox?max_id=01FJC1Q0E3SSQR59TD2M1KP4V8&page=true",
				//	"prev": "https://poppastring.com/users/mdownie/outbox?min_id=01FJC1Q0E3SSQR59TD2M1KP4V8&page=true",
				//	"partOf": "https://poppastring.com/users/mdownie/outbox",
				//	"orderedItems": [

				//		{
				//					"id": "https://poppastring.com/users/mdownie/statuses/01FJC1MKPVX2VMWP2ST93Q90K7/activity",
				//			"type": "Create",
				//			"actor": "https://poppastring.com/users/mdownie",
				//			"published": "2021-10-18T20:06:18Z",
				//			"to": [

				//				"https://www.w3.org/ns/activitystreams#Public"
				//			],
				//			"cc": [

				//				"https://poppastring.com/users/mdownie/followers"
				//			],
				//			"object": "https://poppastring.com/users/mdownie/statuses/01FJC1MKPVX2VMWP2ST93Q90K7"

				//		}
				//	]
				//}

			}
			else
			{
				//{
				//    "@context": "https://www.w3.org/ns/activitystreams",
				//    "id": "https://poppastring.com/users/mdownie/outbox",
				//    "type": "OrderedCollection",
				//    "first": "https://poppastring.com/users/mdownie/outbox?page=true"
				//}

			}

			return Json("");
		}
	}
}
