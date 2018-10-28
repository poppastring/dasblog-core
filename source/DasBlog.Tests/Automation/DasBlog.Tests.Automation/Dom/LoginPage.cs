using DasBlog.Tests.Automation.Common;
using DasBlog.Tests.Automation.Selenium.Interfaces;
using AppConstants = DasBlog.Core.Common.Constants;

namespace DasBlog.Tests.Automation.Dom
{
	public class LoginPage : Page
	{
		public LoginPage(IBrowser browser) : base(browser, Constants.LoginPage, AppConstants.LoginPageTestId)
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

		public TextBoxPageElement EmailTextBox
		{
			get { return browser.GetTextBoxElementById(AppConstants.Email); }
		}
		public TextBoxPageElement PasswordTextBox
		{
			get { return browser.GetTextBoxElementById(AppConstants.Password); }
		}
	}
}
