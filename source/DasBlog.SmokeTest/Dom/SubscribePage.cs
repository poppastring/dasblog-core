using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class SubscribePage : Page
	{
		public SubscribePage(IBrowser browser) : base(browser, Constants.SubscribePage)
		{
		}
	}
}