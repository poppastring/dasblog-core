using DasBlog.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	/// <summary>
	/// How Federated servers communicate with other federated servers only.
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
