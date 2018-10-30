using System;
using System.Collections.ObjectModel;
using System.Linq;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.Support
{
	public class BashScriptPlatform : IScriptPlatform
	{
		private ILogger<BashScriptPlatform> logger;
		public BashScriptPlatform(ILogger<BashScriptPlatform> logger)
		{
			this.logger = logger;
		}

		/// <inheritdoc cref="IScriptPlatform"/>
		public string GetCmdExe(bool suppressLog = false)
		{
			return "bash";
		}

		/// <inheritdoc cref="IScriptPlatform"/>
		public string GetNameAndScriptSubDirectory(string scriptId)
		{
			return $"bash/{scriptId}.sh";
		}
		/// <inheritdoc cref="IScriptPlatform"/>
		public string[] GetShellFlags()
		{
			return new [] {"-c"};
		}

		/// <inheritdoc cref="IScriptPlatform"/>
		public void GatherArgsForPsi(Collection<string> psiArgumentList
			, object[] shellFlags, string scriptPathAndName, object[] scriptArgs)
		{
			foreach (var arg in shellFlags)
			{
				psiArgumentList.Add((string)arg);
			}

			var shellArg = string.Join(" ", scriptArgs.Select(a => $"'{a}'"));
			psiArgumentList.Add($"'{scriptPathAndName}' {shellArg}");
		}

	}
}
