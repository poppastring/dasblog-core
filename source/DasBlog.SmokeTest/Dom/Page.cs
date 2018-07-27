namespace DasBlog.SmokeTest.Dom
{
	public class Page
	{
		public Pages Pages { get; private set; }
		private readonly string path;						// relative to the root e.g. "category" or "account/login"
		public Page(Pages pages, string path)
		{
			this.Pages = pages;
			this.path = path;
		}
		public void Goto()
		{
			Pages.Browser.Goto(path);
		}

	}
}
