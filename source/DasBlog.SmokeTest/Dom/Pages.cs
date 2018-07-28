using System.Globalization;
using System.Runtime.CompilerServices;
using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class Pages
	{
		public IBrowser Browser { get; private set; }
		public Pages(IBrowser browser)
		{
			this.Browser = browser;
			this.NavBar = new NavBarPage(browser);
			this.Home = new HomePage(browser);
			this.Category = new CategoryPage(browser);
			this.Archive = new ArchivePage(browser);
			this.Subscribe = new SubscribePage(browser);
			this.PostMaintenance = new PostMaintenancePage(browser);
			this.Users = new UsersPage(browser);
			this.Activity = new ActivityPage(browser);
			this.Http404 = new Http404Page(browser);
			this.Login = new LoginPage(browser);
		}


		public NavBarPage NavBar { get; private set; }
		public HomePage Home { get; private set; }
		public CategoryPage Category { get; private set; }
		public ArchivePage Archive { get; private set; }
		public SubscribePage Subscribe { get; private set; }
		public PostMaintenancePage PostMaintenance { get; private set; }
		public UsersPage Users { get; private set; }
		public ActivityPage Activity { get; private set; }
		public Http404Page Http404 { get; private set; }
		public LoginPage Login { get; private set; }
	}

	public class LoginPage : Page
	{
		public LoginPage(IBrowser browser) : base(browser, Constants.LoginPage)
		{
		}

		public ButtonElement LoginButton { get; set; }
		public TextElement Password { get; set; }

		public bool IsDisplayed()
		{
			return false;
		}
	}

	public class TextElement
	{
		public string Text { get; set; }
	}

	public class ButtonElement
	{
		public void Click()
		{
		}
	}

	public class Http404Page :  Page
	{
		public Http404Page(IBrowser browser) : base(browser, "somerubbish")
		{
			
		}
	}

	public class ActivityPage : Page
	{
		public ActivityPage(IBrowser browser) : base(browser, Constants.ActivityPage)
		{

		}
	}

	public class UsersPage : Page
	{
		public UsersPage(IBrowser browser) : base(browser, Constants.UsersPage)
		{
		}
	}

	public class PostMaintenancePage : Page
	{
		public PostMaintenancePage(IBrowser browser) : base(browser, Constants.PostMaintenancePage)
		{
		
		}
	}

	public class SubscribePage : Page
	{
		public SubscribePage(IBrowser browser) : base(browser, Constants.SubscribePage)
		{
		}
	}

	public class ArchivePage : Page
	{
		public ArchivePage(IBrowser browser) : base(browser, Constants.ArchivePage)
		{
		}
	}

	public class CategoryPage : Page
	{
		public CategoryPage(IBrowser browser) : base(browser, Constants.CategoryPage)
		{
		}
	}

	public class NavBarPage : Page
	{
		public NavBarPage(IBrowser browser) : base(browser, Constants.NavBarPage)
		{
		}
	}
}
