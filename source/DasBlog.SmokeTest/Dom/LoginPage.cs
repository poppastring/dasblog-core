using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Interfaces;

namespace DasBlog.SmokeTest.Dom
{
	public class LoginPage : Page
	{
		public LoginPage(IBrowser browser) : base(browser, Constants.LoginPage, Constants.LoginPageTitle)
		{
		}

		public ButtonElement LoginButton
		{
			get
			{
				return browser.GetButtonById("loginButton");
			}
		}

		public SpanElement Password
		{
			get { return browser.GetElementById("passwordValidation"); }
		}
	}
}
