using OpenQA.Selenium;

namespace DasBlog.SmokeTest.Dom
{
	public abstract class PageElement
	{
		public IWebElement WebElement { protected get; set; }
	}
}
