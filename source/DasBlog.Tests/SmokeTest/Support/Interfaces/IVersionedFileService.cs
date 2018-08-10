namespace DasBlog.Tests.SmokeTest.Support.Interfaces
{
	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();
		(bool clean, string errorMessage) IsClean();
		void Restore();
	}
}
