using OpenQA.Selenium;

namespace DasBlog.Tests.Automation.Dom
{
	public abstract class PageElement
	{
		public IWebElement WebElement { get; set; }
		public string Id { get; set; }
		// store the id locally - using the attribute of the web element is fraught - remoting gets in the way
	}
}
