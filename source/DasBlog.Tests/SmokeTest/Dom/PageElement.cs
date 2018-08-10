using OpenQA.Selenium;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public abstract class PageElement
	{
		public IWebElement WebElement { protected get; set; }
	}
}
