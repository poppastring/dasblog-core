using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class CategoryPage : Page
	{
		public CategoryPage(IBrowser browser) : base(browser, Constants.CategoryPage, Core.Common.Constants.CategoryPageTitle)
		{
		}
	}
}