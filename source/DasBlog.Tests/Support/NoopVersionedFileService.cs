using DasBlog.Tests.Support.Interfaces;

namespace DasBlog.Tests.Support
{
	public class NoopVersionedFileService : IVersionedFileService
	{
		public (bool active, string errorMessage) IsActive()
		{
			return (true, string.Empty);
		}

		public (bool clean, string errorMessage) IsClean(string environment, bool suppressLog)
		{
			return (true, string.Empty);
		}

		public void StashCurrentStateIfDirty(string environment, bool suppressLog)
		{
		}

		public string TestDataPath { get; } = string.Empty;
	}
}
