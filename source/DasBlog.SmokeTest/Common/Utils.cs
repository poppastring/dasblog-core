using System.Runtime.CompilerServices;

namespace DasBlog.SmokeTest.Common
{
	public static class Utils
	{
		public static string GetMethodName([CallerMemberName] string methodName = null)
		{
			return methodName;
		}
	}
}
