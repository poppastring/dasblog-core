using System.Threading;

namespace DasBlog.Tests.Support
{
	internal class WaitService
	{
		private ManualResetEvent @event =  new ManualResetEvent(true);

		public void Wait()
		{
			@event.WaitOne();
		}

		public void StopWaiting()
		{
			@event.Set();
		}
	}
}
