using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.Support
{
	public class ScriptRunner : IScriptRunner
	{
		public IReadOnlyDictionary<string, string> DefaultEnv => defaultEnv;
		private readonly Dictionary<String, string> defaultEnv = new Dictionary<string, string>(); 
		private ILogger<ScriptRunner> logger;
		private readonly string scriptDirectory;
		private readonly int scriptTimeout = 5_000;
		private readonly int scriptExitTimeout = 10;

		public ScriptRunner(IOptions<ScriptRunnerOptions> opts, ILogger<ScriptRunner> logger)
		{
			this.scriptDirectory = opts.Value.ScriptDirectory;
			this.scriptTimeout = opts.Value.ScriptTimeout;
			this.scriptExitTimeout = opts.Value.ScriptExitTimeout;
			this.logger = logger;
		}

		/// <inheritdoc cref="DasBlog.Tests.Support.Interfaces"/>
		/// some unpleasantness with capturing output
		/// The following applied when only "echo" statements appeared in the script.
		/// cmd /C - no output was returned
		/// cmd /K with "exit" in the script - no output was returned
		/// cmd /K with "exit /b" in the script - output was returned but the process timed out.
		/// added the statement "set" on the line after "echo" and the echoed expressions appeared.
		/// conclusion: the script exits before the process has an opportunity to get the output
		/// DON'T have scripts that comprise only "echo" statements
		public (int exitCode, string[] output, string[] errors) Run(
			string scriptName, IReadOnlyDictionary<string, string> envirnmentVariables,
			params object[] arguments)
		{
			try
			{
				var cmdexe = GetCmdExe();
				var output = new List<string>();
				var errs = new List<string>();
				
				ProcessStartInfo psi = new ProcessStartInfo(cmdexe);
				var scriptPathAndFileName = Path.Combine(scriptDirectory, scriptName);
				psi.UseShellExecute = false;
				SetArguments(psi.ArgumentList
					, new string[]
					{
						"/K", scriptPathAndFileName, scriptExitTimeout.ToString()
					}.Concat(arguments).ToArray());
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardError = true;
				logger.LogInformation($"script timeout: {scriptTimeout}, script exit timeout {scriptExitTimeout}");

				int exitCode = int.MaxValue;
				using (var ps = Process.Start(psi))
				{
					ps.OutputDataReceived += (sender, e) => output.Add(e.Data);
					ps.ErrorDataReceived += (sender, e) => errs.Add(e.Data);
					ps.BeginOutputReadLine();
					ps.BeginErrorReadLine();
					var result = ps.WaitForExit(scriptTimeout);
					exitCode = result ? ps.ExitCode : int.MaxValue - 1;
				}
				logger.LogInformation($"exit code: {exitCode}");

				ThrowExceptionForBadExitCode(exitCode, scriptPathAndFileName, scriptTimeout, psi);
				return (exitCode, output.ToArray(), errs.ToArray());
			}
//			finally
			catch (Exception e)
			{
				throw new Exception(e.Message, e);
			}
		}

		private void ThrowExceptionForBadExitCode(int exitCode, string scriptName, int timeout, ProcessStartInfo psi)
		{
			string message = string.Empty;
			switch (exitCode)
			{
				case Constants.ScriptTimedOutCode:
					message =
						$"Execution of the script timed out after {timeout} milliseconds: {scriptName}" 
						+ Environment.NewLine + "This can be set (in milliseconds) through the environment variable "
						+ "DAS_BLOG_TEST_SCRIPT_TIMEOUT";
					break;
				case Constants.ScriptProcessFailedToRunCode:
					var cmdLine = psi.FileName + " " + string.Join(' ', psi.ArgumentList);
					message = $"failed to run the command line: {cmdLine}";
					break;
				default:
					// other exit codes are handled by the caller to Run
					return;
			}
			throw new Exception(message);
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
		/// <summary>
		/// full path to the scripts directory
		/// </summary>
		public string ScriptDirectory { get; set; }
		/// <summary>
		/// timeout in milliseconds
		/// </summary>
		public int ScriptTimeout { get; set; }
		/// <summary>
		/// timeout in milliseconds
		/// </summary>
		public int ScriptExitTimeout { get; set; }
	}
}
