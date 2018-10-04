using WebAppConstants = DasBlog.Core.Common.Constants;
using DasBlog.Tests.Automation.Selenium.Interfaces;

namespace DasBlog.Tests.Automation.Dom
{
	public class ArchivePage : Page
	{
		public ArchivePage(IBrowser browser) : base(browser, "archive", WebAppConstants.ArchivePageTestId)
		{
		}
	}
}
