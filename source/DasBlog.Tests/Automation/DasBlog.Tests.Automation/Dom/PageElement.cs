using OpenQA.Selenium;

namespace DasBlog.Tests.Automation.Dom
{
	public abstract class PageElement
	{
		public IWebElement WebElement { protected get; set; }
	}
}
