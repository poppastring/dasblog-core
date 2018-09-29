using DasBlog.Tests.Support.Interfaces;

namespace DasBlog.Tests.Support
{
	public class NoopVersionedFileService : IVersionedFileService
	{
		public (bool active, string errorMessage) IsActive()
		{
			return (true, string.Empty);
		}

		public (bool clean, string errorMessage) IsClean(string environment)
		{
			return (true, string.Empty);
		}

		public void StashCurrentState(string environment)
		{
		}

	}
}
