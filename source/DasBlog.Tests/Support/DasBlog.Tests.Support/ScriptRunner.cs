using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using DasBlog.Tests.SmokeTest.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.Support
{
	public class ScriptRunner : IScriptRunner
	{
		private ILogger<ScriptRunner> logger;
		private string scriptDirectory;

		public ScriptRunner(IOptions<ScriptRunnerOptions> opts, ILogger<ScriptRunner> logger)
		{
			this.scriptDirectory = opts.Value.ScriptDirectory;
			this.logger = logger;
		}

		/// <inheritdoc cref="DasBlog.Tests.Support.Interfaces"/>
		public (int exitCode, string[] output, string[] errors) Run(
			string scriptName, IReadOnlyDictionary<string, string> envirnmentVariables,
			params object[] arguments)
		{
			var cmdexe = GetCmdExe();
			StringBuilder output = new StringBuilder();
			StringBuilder errs = new StringBuilder();
			ProcessStartInfo psi = new ProcessStartInfo(cmdexe);
			var scriptPathAndFileName = Path.Combine(scriptDirectory, scriptName);
//			var cmdLineArgs = string.Join(' ', arguments.Select(a => $@"""{a}"")").ToList());
			psi.UseShellExecute = false;
//			psi.Arguments =
//				$@"/Q /U /K ""{criptPathAndFileName}"" " + cmdLineArgs;
			SetArguments(psi.ArgumentList
			  , new string[] {"/Q", "/U", "/K", scriptPathAndFileName}.Concat(arguments).ToArray());
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			int exitCode = int.MaxValue;
			using (var ps = Process.Start(psi))
			{
				ps.OutputDataReceived += (sender, e) => output.Append(e.Data);
				ps.ErrorDataReceived += (sender, e) => errs.Append(e.Data);
				ps.BeginOutputReadLine();
				ps.BeginErrorReadLine();
				ps.WaitForExit();
				exitCode = ps.ExitCode;
			}

			return (exitCode, output.ToString().Split(Environment.NewLine), errs.ToString().Split(Environment.NewLine));
		}

		private void SetArguments(Collection<string> psiArgumentList, object[] arguments)
		{
			foreach (var arg in arguments)
			{
				psiArgumentList.Add((string)arg);
			}
		}

		private string GetCmdExe()
		{
			var cmdexe = Environment.GetEnvironmentVariable("ComSpec");
			if (string.IsNullOrWhiteSpace(cmdexe))
			{
				logger.LogInformation("comspec environment variable was empty - will use cmd.exe");
				return "cmd.exe";
			}

			return cmdexe;
		}
	}

	public class ScriptRunnerOptions
	{
		public string ScriptDirectory { get; set; }
	}
}
