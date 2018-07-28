using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class Page
	{
		protected readonly IBrowser browser;
		protected readonly string path;						// relative to the root e.g. "category" or "account/login"
		protected readonly string title;
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

		public bool IsDisplayed()
		{
			return browser.GetTitle() == title;
		}
	}
}
