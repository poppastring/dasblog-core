using OpenQA.Selenium;

namespace DasBlog.SmokeTest.Dom
{
	public abstract class Element
	{
		protected readonly IWebElement webElement;

		public Element(IWebElement webElement)
		{
			System.Diagnostics.Debug.Assert(webElement != null);
			this.webElement = webElement;
			
		}
	}
}