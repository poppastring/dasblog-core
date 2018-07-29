using System;
using DasBlog.SmokeTest.Smoking;

namespace DasBlog.SmokeTest.Smoking.Interfaces
{
	public interface ITester : IDisposable
	{
		void Test();
		TestResults Results { get; }
	}
}
