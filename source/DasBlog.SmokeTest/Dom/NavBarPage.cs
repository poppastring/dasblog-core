using System.Collections.Generic;
using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.SmokeTest.Dom
{
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

				LinkElement el = browser.GetLinkById(Core.Common.Constants.CategoryId);
				if (el != null)
				{
					elements[id] = el;
				}
				return el;
			}
		}

		public override bool IsDisplayed()
		{
			return browser.GetSomeElementId(AppConstants.NavBarId) != null;
		}
	}
}
