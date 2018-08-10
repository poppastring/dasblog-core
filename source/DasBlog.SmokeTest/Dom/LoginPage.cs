using DasBlog.SmokeTest.Common;
using DasBlog.SmokeTest.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.SmokeTest.Dom
{
	public class LoginPage : Page
	{
		public LoginPage(IBrowser browser) : base(browser, Constants.LoginPage, Constants.LoginPageTitle)
		{
		}

		public ButtonPageElement LoginButton
		{
			get
			{
				return browser.GetButtonById(AppConstants.LoginButtonId);
			}
		}

		public SpanPageElement PasswordValidation
		{
			get { return browser.GetElementById(AppConstants.PasswordValidationId); }
		}
	}
}
