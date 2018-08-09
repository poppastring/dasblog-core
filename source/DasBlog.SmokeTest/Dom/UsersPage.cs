using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class UsersPage : Page
	{
		public UsersPage(IBrowser browser) : base(browser, Constants.UsersPage)
		{
		}
	}
}