using System;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.Support
{
	public class CmdScriptPlatform : IScriptPlatform
	{
		private ILogger<CmdScriptPlatform> logger;
		public CmdScriptPlatform(ILogger<CmdScriptPlatform> logger)
		{
			this.logger = logger;
		}
		public string GetCmdExe(bool suppressLog = false)
		{
			var cmdexe = Environment.GetEnvironmentVariable("ComSpec");
			if (string.IsNullOrWhiteSpace(cmdexe))
			{
				if (!suppressLog) logger.LogInformation("comspec environment variable was empty - will use cmd.exe");
				return "cmd.exe";
			}

			return cmdexe;
		}

		public string GetNameAndScriptSubDirectory(string scriptId)
		{
			return $"cmd/{scriptId}.cmd";
		}
	}
}
