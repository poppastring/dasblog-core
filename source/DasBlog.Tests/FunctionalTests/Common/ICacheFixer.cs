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
		/// increments the change on the DataManager which eventually causes the day headers to be reloaded
		/// and the EntryCache and CatagoryCache to be reloaded.
		/// </summary>
		/// <param name="blogManager">root of object tree in order to discover necessary fields and method</param>
		void InvalidateCache(IBlogManager blogManager);
	}
}
