using System;

namespace DasBlog.SmokeTest.Interfaces
{
	public interface IBrowser : IDisposable
	{
		void Goto(string path);
	}
}
