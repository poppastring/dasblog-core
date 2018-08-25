using System;
using DasBlog.Tests.Support;

namespace DasBlog.Tests.SmokeTest
{
	public interface ITester : IDisposable
	{
		void Test();
		TestResults Results { get; }
	}
}
