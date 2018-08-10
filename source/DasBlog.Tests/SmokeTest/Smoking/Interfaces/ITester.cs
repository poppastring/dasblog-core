using System;

namespace DasBlog.Tests.SmokeTest.Smoking.Interfaces
{
	public interface ITester : IDisposable
	{
		void Test();
		TestResults Results { get; }
	}
}
