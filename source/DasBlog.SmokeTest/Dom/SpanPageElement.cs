using OpenQA.Selenium;

namespace DasBlog.SmokeTest.Dom
{
	public class SpanPageElement : PageElement
	{
		public string Text
		{
			get { return WebElement.Text; }
		}
	}
}
