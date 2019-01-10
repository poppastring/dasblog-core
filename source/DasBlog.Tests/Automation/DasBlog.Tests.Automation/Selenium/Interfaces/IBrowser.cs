using System;
using DasBlog.Tests.Automation.Dom;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.Automation.Selenium.Interfaces
{
	public interface IBrowser : IDisposable
	{
		void Goto(string path);
		void Init();
		string GetTitle();
		string GetUrl();
		ButtonPageElement GetButtonById(string id);
		SpanPageElement GetElementById(string id);
		TextBoxPageElement GetTextBoxElementById(string id);
		LinkPageElement GetLinkById(string categoryId);
		AnyPageElement GetAnyElementById(string navBarId);
		DivPageElement GetDivById(string pageTestId);
		string GetPageSource();
		bool IsElementVisible(PageElement pe);
		ILogger<Browser> Logger { get; }
	}
}
