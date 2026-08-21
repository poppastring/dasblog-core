using DasBlog.Services.Atproto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.Controllers
{
	[Produces("text/plain")]
	public class WellKnownController : ControllerBase
	{
		private readonly IAtprotoPublisher atprotoPublisher;
		private readonly ILogger<WellKnownController> logger;

		public WellKnownController(IAtprotoPublisher atprotoPublisher, ILogger<WellKnownController> logger)
		{
			this.atprotoPublisher = atprotoPublisher;
			this.logger = logger;
		}

		[HttpGet("/.well-known/site.standard.publication")]
		public ActionResult AtprotoPublication()
		{
			var publicationUri = atprotoPublisher.GetPublicationUri();

			if (string.IsNullOrWhiteSpace(publicationUri))
			{
				logger.LogDebug("ATProto publication URI not available.");
				return new StatusCodeResult(404);
			}

			return new ContentResult
			{
				Content = publicationUri,
				ContentType = "text/plain; charset=utf-8",
				StatusCode = 200
			};
		}
	}
}
