using OpenQA.Selenium;

namespace DasBlog.SmokeTest.Dom
{
	public class AnyElement : Element
	{
		public AnyElement(IWebElement webElement) : base(webElement)
		{
		}
	}
}
