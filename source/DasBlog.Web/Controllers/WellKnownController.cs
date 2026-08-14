using DasBlog.Services.Atproto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.Controllers
{
	[ApiController]
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
		public async Task<ActionResult> AtprotoPublication()
		{
			var publicationUri = await atprotoPublisher.EnsurePublicationAsync();

			if (string.IsNullOrWhiteSpace(publicationUri))
			{
				logger.LogWarning("ATProto publication URI not available.");
				return NotFound();
			}

			return Content(publicationUri, "text/plain", Encoding.UTF8);
		}
	}
}
