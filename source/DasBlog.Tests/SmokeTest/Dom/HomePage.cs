using WebAppConstants = DasBlog.Core.Common.Constants;
using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class HomePage : Page
	{
		public HomePage(IBrowser browser) : base(browser, Constants.HomePage, WebAppConstants.HomePageTitle)
		{
			
		}

	}
}
