using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.SmokeTest.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.SmokeTest.Dom
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
