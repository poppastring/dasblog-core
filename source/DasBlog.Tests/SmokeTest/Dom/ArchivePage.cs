using WebAppConstants = DasBlog.Core.Common.Constants;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class ArchivePage : Page
	{
		public ArchivePage(IBrowser browser) : base(browser, "archive", WebAppConstants.ArchivePageTitle)
		{
		}
	}
}
