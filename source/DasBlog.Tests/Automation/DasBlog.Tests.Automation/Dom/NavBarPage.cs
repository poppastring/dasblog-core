using System.Collections.Generic;
using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.Automation.Dom
{
	public class NavBarPage : Page
	{
		Dictionary<string, LinkPageElement> elements = new Dictionary<string, LinkPageElement>();
		public NavBarPage(IBrowser browser) : base(browser, Constants.NavBarPage, "there is no page test id for navbar")
		{
		}

		/// <summary>
		/// returns a link that the tester can call Click on
		/// The caller should test this for null before proceding with click etc.
		/// </summary>
		/// <param name="id">something like "CategoryNavBarId"
		/// 	See navbar link ids in Constants
		/// </param>
		public LinkPageElement this[string id]
		{
			get
			{
				if (elements.ContainsKey(id))
				{
					return elements[id];
				}

				LinkPageElement el = browser.GetLinkById(id);
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
