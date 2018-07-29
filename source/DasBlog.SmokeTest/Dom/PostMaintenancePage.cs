using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class PostMaintenancePage : Page
	{
		public PostMaintenancePage(IBrowser browser) : base(browser, Constants.PostMaintenancePage)
		{
		
		}
	}
}