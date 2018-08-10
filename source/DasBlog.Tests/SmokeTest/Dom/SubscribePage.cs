using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class SubscribePage : Page
	{
		public SubscribePage(IBrowser browser) : base(browser, Constants.SubscribePage)
		{
		}
	}
}