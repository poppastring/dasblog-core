using OpenQA.Selenium;

namespace DasBlog.SmokeTest.Dom
{
	public class ButtonElement : Element
	{
		public ButtonElement(IWebElement webElement) : base(webElement)
		{
		}
		public void Click()
		{
			webElement.Click();
		}
	}
}