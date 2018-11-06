using System;
using System.Collections.ObjectModel;
using System.Linq;
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
		/// <inheritdoc cref="IScriptPlatform"/>
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

		/// <inheritdoc cref="IScriptPlatform"/>
		public string GetNameAndScriptSubDirectory(string scriptId)
		{
			return $"cmd/{scriptId}.cmd";
		}

		/// <inheritdoc cref="IScriptPlatform"/>
		public string[] GetShellFlags()
		{
			return new [] {"/K"};
		}

		/// <inheritdoc cref="IScriptPlatform"/>
		public void GatherArgsForPsi(Collection<string> psiArgumentList, object[] shellFlags, string scriptPathAndName, object[] scriptArgs)
		{
			foreach (var arg in shellFlags.Append(scriptPathAndName).Concat(scriptArgs))
			{
				psiArgumentList.Add((string)arg);
			}
		}
	}
}
