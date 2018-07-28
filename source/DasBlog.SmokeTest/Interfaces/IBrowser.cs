using System;
using DasBlog.SmokeTest.Dom;

namespace DasBlog.SmokeTest.Interfaces
{
	public interface IBrowser : IDisposable
	{
		void Goto(string path);
		void Init();
		string GetTitle();
		ButtonElement GetButtonById(string id);
		SpanElement GetElementById(string id);
	}
}
