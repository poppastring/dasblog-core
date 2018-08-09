using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class HomePage : Page
	{
		public HomePage(IBrowser browser) : base(browser, Constants.HomePage)
		{
			
		}

	}
}
