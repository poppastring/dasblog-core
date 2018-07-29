using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class ActivityPage : Page
	{
		public ActivityPage(IBrowser browser) : base(browser, Constants.ActivityPage)
		{

		}
	}
}