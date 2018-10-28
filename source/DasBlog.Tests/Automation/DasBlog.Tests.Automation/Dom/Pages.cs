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


		public NavBarPage NavBar { get; }
		public HomePage HomePage { get; }
		public CategoryPage CategoryPage { get; }
		public ArchivePage ArchivePage { get; }
		public SubscribePage SubscribePage { get; }
		public PostMaintenancePage PostMaintenancePage { get; }
		public UsersPage UsersPage { get; }
		public ActivityPage ActivityPage { get; }
		public Http404Page Http404Page { get; }
		public LoginPage LoginPage { get; }
	}
}
