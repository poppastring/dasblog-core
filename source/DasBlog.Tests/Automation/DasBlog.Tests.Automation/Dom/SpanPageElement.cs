using OpenQA.Selenium;

namespace DasBlog.Tests.Automation.Dom
{
	public class SpanPageElement : PageElement
	{
		public string Text
		{
			get { return WebElement.Text; }
		}
	}
}
