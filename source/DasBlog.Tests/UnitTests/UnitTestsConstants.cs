using System.IO;

namespace DasBlog.Tests.UnitTests
{
	public class UnitTestsConstants
	{
		private static readonly DirectoryInfo root = new DirectoryInfo(GetProjectBinaryDirectory());

		public static string TestContentLocation { get { return new DirectoryInfo(Path.Combine(root.Parent.FullName, "netcoreapp3.1/TestContent")).FullName; } }

		public static string TestLoggingLocation { get { return new DirectoryInfo(Path.Combine(root.Parent.FullName, "netcoreapp3.1/logs")).FullName; } }

		public static string GetProjectBinaryDirectory()
		{
			return Path.GetFullPath(Path.GetDirectoryName(typeof(UnitTestsConstants).Assembly.Location));
		}
	}
}
