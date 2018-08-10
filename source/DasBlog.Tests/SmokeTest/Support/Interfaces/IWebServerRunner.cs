namespace DasBlog.Tests.SmokeTest.Support.Interfaces
{
	public interface IWebServerRunner
	{
		void RunDasBlog();
		void Kill();
	}
}
