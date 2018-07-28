using System;

namespace DasBlog.SmokeTest.Interfaces
{
	public interface ITester : IDisposable
	{
		void Test();
		TestResults Results { get; }
	}
}
