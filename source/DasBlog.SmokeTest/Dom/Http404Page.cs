using DasBlog.SmokeTest.Selenium.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class Http404Page :  Page
	{
		public Http404Page(IBrowser browser) : base(browser, "somerubbish")
		{
			
		}
	}
}