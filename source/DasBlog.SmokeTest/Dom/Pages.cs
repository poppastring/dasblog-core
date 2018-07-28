using System.Collections.Generic;
using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;
using OpenQA.Selenium;

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
		Dictionary<string, LinkElement> elements = new Dictionary<string, LinkElement>();
		public NavBarPage(IBrowser browser) : base(browser, Constants.NavBarPage)
		{
		}

		public LinkElement this[string id]
		{
			get
			{
				if (elements.ContainsKey(id))
				{
					return elements[id];
				}

				LinkElement el = browser.GetLinkById(AppConstants.CategoryId);
				if (el != null)
				{
					elements[id] = el;
				}
				return el;
			}
		}
	}

	public class LinkElement : Element
	{
		public LinkElement(IWebElement webElement) : base(webElement)
		{
			
		}

		public void Click()
		{
			webElement.Click();
		}
	}
}
