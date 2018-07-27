using DasBlog.SmokeTest.Interfaces;
using LibGit2Sharp;

namespace DasBlog.SmokeTest.Dom
{
	public class Pages
	{
		public IBrowser Browser { get; private set; }
		public Pages(IBrowser browser)
		{
			this.Browser = browser;
			this.NavBar = new NavBarPage(this);
			this.Home = new HomePage(this);
			this.Category = new CategoryPage(this);
			this.Archive = new ArchivePage(this);
			this.Subscribe = new SubscribePage(this);
			this.PostMaintenance = new PostMaintenancePage(this);
			this.Users = new UsersPage(this);
			this.Activity = new ActivityPage(this);
			this.Http404 = new Http404Page(this);
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
	}

	public class Http404Page :  Page
	{
		public Http404Page(Pages pages) : base(pages, "somerubbish")
		{
			
		}
	}

	public class ActivityPage : Page
	{
		public ActivityPage(Pages pages) : base(pages, Constants.ActivityPage)
		{

		}
	}

	public class UsersPage : Page
	{
		public UsersPage(Pages pages) : base(pages, Constants.UsersPage)
		{
		}
	}

	public class PostMaintenancePage : Page
	{
		public PostMaintenancePage(Pages pages) : base(pages, Constants.PostMaintenancePage)
		{
		
		}
	}

	public class SubscribePage : Page
	{
		public SubscribePage(Pages pages) : base(pages, Constants.SubscribePage)
		{
		}
	}

	public class ArchivePage : Page
	{
		public ArchivePage(Pages pages) : base(pages, Constants.ArchivePage)
		{
		}
	}

	public class CategoryPage : Page
	{
		public CategoryPage(Pages pages) : base(pages, Constants.CategoryPage)
		{
		}
	}

	public class NavBarPage : Page
	{
		public NavBarPage(Pages pages) : base(pages, Constants.NavBarPage)
		{
		}
	}
}
