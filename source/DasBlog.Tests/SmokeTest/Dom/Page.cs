using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class Page
	{
		protected readonly IBrowser browser;
		protected readonly string path;						// relative to the root e.g. "category" or "account/login"
		protected readonly string title;
		// TODO remoe optional from title - two strings / one string is a gotcha
		public Page(IBrowser browser, string path, string title = null)
		{
			this.browser = browser;
			this.path = path;
			this.title = title;
		}
		public void Goto()
		{
			browser.Goto(path);
		}

		public virtual bool IsDisplayed()
		{
			return browser.GetTitle() == title;
		}
	}
}
