using System;
using DasBlog.Tests.SmokeTest.Dom;

namespace DasBlog.Tests.SmokeTest.Selenium.Interfaces
{
	public interface IBrowser : IDisposable
	{
		void Goto(string path);
		void Init();
		string GetTitle();
		ButtonPageElement GetButtonById(string id);
		SpanPageElement GetElementById(string id);
		LinkPageElement GetLinkById(string categoryId);
		AnyPageElement GetAnyElementById(string navBarId);
	}
}
