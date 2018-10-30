using System.IO;
using System.Runtime.CompilerServices;

namespace DasBlog.Tests.Support.Common
{
	public static class Utils
	{
		public static string GetMethodName([CallerMemberName] string methodName = null)
		{
			return methodName;
		}
		/// <summary>
		/// all test paths are relative to this one.
		/// </summary>
		/// <returns>e,g, c:/projects/das-blog/</returns>
		public static string GetProjectRootDirectory()
		{
			return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(typeof(Utils).Assembly.Location),
				Constants.DasBlogProjectRootRelativeToAssemblies));
		}
		/// <summary>
		/// location of the containing assembly
		/// </summary>
		/// <returns>e,g, c:/projects/das-blog/source/Dasblog.Tests/UnitTests/bin/Debug/netappcore2.1
		/// or possibly Functional depending on the caller</returns>
		public static string GetProjectBinaryDirectory()
		{
			return Path.GetFullPath(Path.GetDirectoryName(typeof(Utils).Assembly.Location));
		}
	}
}
