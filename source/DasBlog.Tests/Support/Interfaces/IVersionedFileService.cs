namespace DasBlog.Tests.Support.Interfaces
{
	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();
		/// <summary>
		/// 
		/// </summary>
		/// <param name="environment">e.g. "Vanilla"
		///   must be a subdirectory of ConstantsTestDataDirectory</param>
		/// <returns></returns>
		(bool clean, string errorMessage) IsClean(string environment);

		/// <summary>
		/// saves any changes made to the working directory by the tests
		/// logs instructions for recovery of the changes
		/// clears out the changes reverting to a pristine state
		/// </summary>
		/// <param name="environment">e.g. 'Vanilla"</param>
		/// <returns>if true then errorMessage contains empty string, else some helpful errors</returns>
		void StashCurrentState(string environment);
	}
}
