using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class ArchivePage : Page
	{
		public ArchivePage(IBrowser browser) : base(browser, Constants.ArchivePage)
		{
		}
	}
}