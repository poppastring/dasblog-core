using WebAppConstants = DasBlog.Core.Common.Constants;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class ArchivePage : Page
	{
		public ArchivePage(IBrowser browser) : base(browser, "archive", WebAppConstants.ArchivePageTitle)
		{
		}
	}
}
