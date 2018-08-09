using System.Collections.Generic;
using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.SmokeTest.Dom
{
	public class NavBarPage : Page
	{
		Dictionary<string, LinkPageElement> elements = new Dictionary<string, LinkPageElement>();
		public NavBarPage(IBrowser browser) : base(browser, Constants.NavBarPage)
		{
		}

		public LinkPageElement this[string id]
		{
			get
			{
				if (elements.ContainsKey(id))
				{
					return elements[id];
				}

				LinkPageElement el = browser.GetLinkById(Core.Common.Constants.CategoryId);
				if (el != null)
				{
					elements[id] = el;
				}
				return el;
			}
		}

		public override bool IsDisplayed()
		{
			return browser.GetAnyElementById(AppConstants.NavBarId) != null;
		}
	}
}
