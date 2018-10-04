using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.Automation.Dom
{
	public class UsersPage : Page
	{
		public UsersPage(IBrowser browser) : base(browser, Constants.UsersPage, AppConstants.UsersPageTestId)
		{
		}
	}
}
