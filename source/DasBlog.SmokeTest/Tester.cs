using DasBlog.SmokeTest.Interfaces;
using OpenQA.Selenium.Firefox;

namespace DasBlog.SmokeTest
{
	internal class Tester : ITester
	{
		public void Test()
		{
			using (var driver = new FirefoxDriver())
			{
				driver.Navigate().GoToUrl("http://ibm.com");
			}
		}
	}
}