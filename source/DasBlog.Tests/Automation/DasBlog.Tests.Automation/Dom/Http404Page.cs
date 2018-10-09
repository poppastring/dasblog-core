using DasBlog.Tests.Automation.Selenium.Interfaces;

namespace DasBlog.Tests.Automation.Dom
{
	public class Http404Page :  Page
	{
		public Http404Page(IBrowser browser) : base(browser, "somerubbish", "there is no page test id for the 404 page")
		{
			
		}
	}
}
