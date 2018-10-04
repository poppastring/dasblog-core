using WebAppConstants = DasBlog.Core.Common.Constants;
using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;

namespace DasBlog.Tests.Automation.Dom
{
	public class HomePage : Page
	{
		public HomePage(IBrowser browser) : base(browser, Constants.HomePage, WebAppConstants.HomePageTestId)
		{
			
		}

	}
}
