using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.ViewComponents
{
	public class CookieConsentViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke()
		{
			var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
			var model = new CookieConsentViewModel
			{
				ShowBanner = !consentFeature?.CanTrack ?? false,
				CookieString = consentFeature?.CreateConsentCookie()
			};
			return View(model);
		}
	}

	public class CookieConsentViewModel
	{
		public bool ShowBanner { get; set; }
		public string CookieString { get; set; }
	}
}
