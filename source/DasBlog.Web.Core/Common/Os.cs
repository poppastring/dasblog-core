namespace DasBlog.Core.Common
{
	/// <summary>
	/// caches the operating system in which the container is executing.
	/// Library users may have OS dependent injections
	/// </summary>
	public enum Os
	{
		/// <summary>
		/// Beans typically have an OS of OS.Any
		/// this will match any OS under which the container is executing
		/// </summary>
		Any,
		/// <summary>
		/// Any version of Linux supporting dotnetstandard 2.0
		/// </summary>
		Linux,
		/// <summary>
		/// Any version of Windows supporting dotnetstandard 2.0
		/// </summary>
		Windows,
		/// <summary>
		/// Any MAC version supported by dotnetstandard 2.0
		/// </summary>
		MacOs
	}
}
