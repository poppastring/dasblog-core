using System.Collections.Generic;
using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class NavBarPage : Page
	{
		Dictionary<string, LinkPageElement> elements = new Dictionary<string, LinkPageElement>();
		public NavBarPage(IBrowser browser) : base(browser, Constants.NavBarPage)
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
