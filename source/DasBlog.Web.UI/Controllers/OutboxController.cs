using DasBlog.Services;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	public class OutboxController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public OutboxController(IDasBlogSettings settings) : base(settings)
		{
			dasBlogSettings = settings;
		}

	}
}
