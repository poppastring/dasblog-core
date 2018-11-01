using System;

namespace DasBlog.Tests.Support.Interfaces
{
	public interface IVersionedFileService
	{
		(bool active, string errorMessage) IsActive();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="environment">e.g. "Vanilla"
		///     must be a subdirectory of ConstantsTestDataDirectory</param>
		/// <param name="suppressLog"></param>
		/// <returns></returns>
		(bool clean, string errorMessage) IsClean(string environment, bool suppressLog);

		/// <summary>
		/// saves any changes made to the working directory by the tests
		/// logs instructions for recovery of the changes
		/// clears out the changes reverting to a pristine state
		/// </summary>
		/// <param name="environment">e.g. 'Vanilla"</param>
		/// <param name="suppressLog"></param>
		/// <returns>if true then errorMessage contains empty string, else some helpful errors</returns>
		void StashCurrentStateIfDirty(string environment, bool suppressLog);

		string TestDataPath { get; }
	}
}
