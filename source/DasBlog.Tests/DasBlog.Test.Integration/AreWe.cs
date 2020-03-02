using System;

namespace DasBlog.Test.Integration
{
	public static class AreWe
	{
		public static bool InDockerOrBuildServer
		{
			get
			{
				var retVal = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
				var retVal2 = Environment.GetEnvironmentVariable("AGENT_NAME");
				return (
					 (string.Compare(retVal, bool.TrueString, ignoreCase: true) == 0)
					 ||
					 (string.IsNullOrWhiteSpace(retVal2) == false));
			}
		}
	}

}
