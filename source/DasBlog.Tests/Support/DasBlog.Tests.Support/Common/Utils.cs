using System.Runtime.CompilerServices;

namespace DasBlog.Tests.SmokeTest.Common
{
	public static class Utils
	{
		public static string GetMethodName([CallerMemberName] string methodName = null)
		{
			return methodName;
		}
	}
}
