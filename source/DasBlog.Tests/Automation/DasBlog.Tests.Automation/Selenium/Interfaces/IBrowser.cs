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
		ButtonPageElement GetButtonById(string id);
		SpanPageElement GetElementById(string id);
		TextBoxPageElement GetTextBoxElementById(string id);
		LinkPageElement GetLinkById(string categoryId);
		AnyPageElement GetAnyElementById(string navBarId);
		DivPageElement GetPageTestIdDiv(string pageTestId);
		string GetPageSource();
		ILogger<Browser> Logger { get; }
	}
}
