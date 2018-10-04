using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;

namespace DasBlog.Tests.Automation.Dom
{
	public class Page
	{
		protected readonly IBrowser browser;
		protected readonly string path;						// relative to the root e.g. "category" or "account/login"
		protected readonly string pageTestId;
		// TODO remoe optional from title - two strings / one string is a gotcha
		public Page(IBrowser browser, string path, string pageTestId = null)
		{
			this.browser = browser;
			this.path = path;
			this.pageTestId = pageTestId;
		}
		public void Goto()
		{
			browser.Goto(path);
		}

		public virtual bool IsDisplayed()
		{
			return browser.GetPageTestIdDiv(pageTestId) != null;
		}
	}
}
