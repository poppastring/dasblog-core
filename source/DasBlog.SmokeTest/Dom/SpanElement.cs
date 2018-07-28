using OpenQA.Selenium;

namespace DasBlog.SmokeTest.Dom
{
	public class SpanElement : Element
	{
		public SpanElement(IWebElement webElement) : base(webElement)
		{
		}
		public string Text
		{
			get { return webElement.Text; }
		}
	}
}