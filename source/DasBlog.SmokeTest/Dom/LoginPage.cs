using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

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
				return browser.GetButtonById(AppConstants.LoginButtonId);
			}
		}

		public SpanElement Password
		{
			get { return browser.GetElementById(AppConstants.PasswordValidationId); }
		}
	}
}
