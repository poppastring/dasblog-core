using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class PostMaintenancePage : Page
	{
		public PostMaintenancePage(IBrowser browser) : base(browser, Constants.PostMaintenancePage)
		{
		
		}
	}
}