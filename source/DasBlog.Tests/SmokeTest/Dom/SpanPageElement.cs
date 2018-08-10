using OpenQA.Selenium;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class SpanPageElement : PageElement
	{
		public string Text
		{
			get { return WebElement.Text; }
		}
	}
}
