using DasBlog.Tests.Automation.Selenium.Interfaces;

namespace DasBlog.Tests.Automation.Dom
{
	public class Pages
	{
		public IBrowser Browser { get; private set; }
		public Pages(IBrowser browser)
		{
			this.Browser = browser;
			this.NavBar = new NavBarPage(browser);
			this.HomePage = new HomePage(browser);
			this.CategoryPage = new CategoryPage(browser);
			this.ArchivePage = new ArchivePage(browser);
			this.SubscribePage = new SubscribePage(browser);
			this.PostMaintenancePage = new PostMaintenancePage(browser);
			this.UsersPage = new UsersPage(browser);
			this.ActivityPage = new ActivityPage(browser);
			this.Http404Page = new Http404Page(browser);
			this.LoginPage = new LoginPage(browser);
		}


		public NavBarPage NavBar { get; private set; }
		public HomePage HomePage { get; private set; }
		public CategoryPage CategoryPage { get; private set; }
		public ArchivePage ArchivePage { get; private set; }
		public SubscribePage SubscribePage { get; private set; }
		public PostMaintenancePage PostMaintenancePage { get; private set; }
		public UsersPage UsersPage { get; private set; }
		public ActivityPage ActivityPage { get; private set; }
		public Http404Page Http404Page { get; private set; }
		public LoginPage LoginPage { get; private set; }
	}
}
