using DasBlog.SmokeTest.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class Page
	{
		private readonly IBrowser browser;
		private readonly string path;						// relative to the root e.g. "category" or "account/login"
		public Page(IBrowser browser, string path)
		{
			this.browser = browser;
			this.path = path;
		}
		public void Goto()
		{
			browser.Goto(path);
		}

	}
}
