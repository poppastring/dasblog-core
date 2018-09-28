namespace DasBlog.Tests.Support.Interfaces
{
	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();
		(bool clean, string errorMessage) IsClean();
		/// <summary>
		/// saves any changes made to the working directory by the tests
		/// logs instructions for recovery of the changes
		/// clears out the changes reverting to a pristine state
		/// </summary>
		/// <returns>if true then errorMessage contains empty string, else some helpful errors</returns>
		void StashCurrentState();
	}
}
