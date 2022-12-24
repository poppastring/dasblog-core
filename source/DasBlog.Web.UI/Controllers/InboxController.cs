using DasBlog.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	/// <summary>
	/// How Federated servers communicate with other federated servers only.
	/// The Inbox stream contains all activities received by the actor.
	/// </summary>
	public class InboxController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public InboxController(IDasBlogSettings settings) : base(settings)
		{
			dasBlogSettings = settings;
		}
	}
}
