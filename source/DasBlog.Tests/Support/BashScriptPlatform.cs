using System;
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
		public string GetCmdExe(bool suppressLog = false)
		{
			return "bash";
		}

		public string GetNameAndScriptSubDirectory(string scriptId)
		{
			return $"bash/{scriptId}.sh";
		}
	}
}
