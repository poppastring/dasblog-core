using DasBlog.Tests.SmokeTest.Selenium.Interfaces;

namespace DasBlog.Tests.SmokeTest.Dom
{
	public class Http404Page :  Page
	{
		public Http404Page(IBrowser browser) : base(browser, "somerubbish")
		{
			
		}
	}
}