using System.IO;

namespace DasBlog.Tests.UnitTests
{
	public class UnitTestsConstants
	{
		private static readonly DirectoryInfo root = new DirectoryInfo(DasBlog.Tests.Support.Common.Utils.GetProjectBinaryDirectory());

		public static string TestContentLocation { get { return new DirectoryInfo(Path.Combine(root.Parent.FullName, "netcoreapp2.1/TestContent")).FullName; } }

		public static string TestLoggingLocation { get { return new DirectoryInfo(Path.Combine(root.Parent.FullName, "netcoreapp2.1/logs")).FullName; } }
	}
}
