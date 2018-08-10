using DasBlog.SmokeTest.Common;
using WebAppConstants = DasBlog.Core.Common.Constants;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class HomePage : Page
	{
		public HomePage(IBrowser browser) : base(browser, Constants.HomePage, WebAppConstants.HomePageTitle)
		{
			
		}

	}
}
