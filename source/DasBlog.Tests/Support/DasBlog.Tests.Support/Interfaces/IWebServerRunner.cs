namespace DasBlog.Tests.Support.Interfaces
{
	public interface IWebServerRunner
	{
		void RunDasBlog();
		void Kill();
	}
}
