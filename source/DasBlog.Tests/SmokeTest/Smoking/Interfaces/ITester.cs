using System;
using DasBlog.Tests.Support;

namespace DasBlog.Tests.SmokeTest.Smoking.Interfaces
{
	public interface ITester : IDisposable
	{
		void Test();
		TestResults Results { get; }
	}
}
