using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;

namespace DasBlog.Tests.Automation.Dom
{
	public class CategoryPage : Page
	{
		public CategoryPage(IBrowser browser) : base(browser, Constants.CategoryPage, Core.Common.Constants.CategoryPageTestId)
		{
		}
	}
}
