using DasBlog.Managers.Interfaces;

namespace DasBlog.Tests.FunctionalTests.Common
{
	/// <summary>
	/// the BlogDataService does not expose a method to invalidate the cache
	/// so we use reflection to do so.
	/// </summary>
	public interface ICacheFixer
	{
		/// <summary>
		/// To handle changes in the content directory made directly by the test routines
		/// this routine increments the change number on the DataManager which eventually causes the day headers to be reloaded
		/// and the EntryCache and CatagoryCache to be reloaded.
		/// 
		/// Where the location of the content directory changes (a new BlogDataService will be createdd)
		/// then we in addition to the above the changeEntryNumber on the CategoryCache is set to -1
		/// as that cache relies on a static value that is not reset by the creation of a new BlogDataService instance
		/// </summary>
		/// <param name="blogManager">root of object tree in order to discover necessary fields and method</param>
		void InvalidateCache(IBlogManager blogManager);
	}
}
