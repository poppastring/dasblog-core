namespace DasBlog.Tests.Automation.Dom
{
	public class TextBoxPageElement : PageElement
	{
		public void SetText(string text)
		{
			WebElement.Clear();
			WebElement.SendKeys(text);
		}
		public string Text
		{
			get => WebElement.Text;
			private set {}
					// expression trees - as used by TestExecutor cannot hack assignment expressions
		}
	}
}
