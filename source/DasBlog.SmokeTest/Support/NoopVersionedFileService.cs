using DasBlog.SmokeTest.Support.Interfaces;

namespace DasBlog.SmokeTest.Support
{
	public class NoopVersionedFileService : IVersionedFileService
	{
		public (bool active, string errorMessage) IsActive()
		{
			return (true, string.Empty);
		}

		public (bool clean, string errorMessage) IsClean()
		{
			return (true, string.Empty);
		}

		public void Restore()
		{
		}
	}
}
