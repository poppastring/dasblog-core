using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
		private readonly IScriptPlatform scriptPlatform;

		public ScriptRunner(IOptions<ScriptRunnerOptions> opts, ILogger<ScriptRunner> logger
		  , IScriptPlatform scriptPlatform)
		{
			this.scriptDirectory = opts.Value.ScriptDirectory;
			this.scriptTimeout = opts.Value.ScriptTimeout;
			this.scriptExitTimeout = opts.Value.ScriptExitTimeout;
			this.scriptPlatform = scriptPlatform;
			this.logger = logger;
		}

		/// <inheritdoc cref="DasBlog.Tests.Support.Interfaces"/>
		public (int exitCode, string[] output) Run(string scriptId,
			IReadOnlyDictionary<string, string> envirnmentVariables,
			bool suppressLog,
			params object[] arguments)
		{
			try
			{
				var cmdexe = GetCmdExe(suppressLog);
				var output = new List<string>();
				var errs = new List<string>();
				var sw = new Stopwatch();
				sw.Start();
				
				ProcessStartInfo psi = new ProcessStartInfo(cmdexe);
				var scriptPathAndFileName = Path.Combine(scriptDirectory
				  , scriptPlatform.GetNameAndScriptSubDirectory(scriptId));
				var shellFlags = scriptPlatform.GetShellFlags();
				psi.UseShellExecute = false;
				scriptPlatform.GatherArgsForPsi(psi.ArgumentList, shellFlags, scriptPathAndFileName, arguments);
				psi.RedirectStandardOutput = true;
				if (!suppressLog) logger.LogDebug($"script timeout: {scriptTimeout}, script exit delay {scriptExitTimeout}ms for {scriptPathAndFileName}");

				var exitCode = RunCmdProcess(psi, output, suppressLog);
				if (!suppressLog) logger.LogDebug($"elapsed time {sw.ElapsedMilliseconds} on thread {Thread.CurrentThread.ManagedThreadId}");
				
				ThrowExceptionForBadExitCode(exitCode, scriptPathAndFileName, scriptTimeout, psi);
				ThrowExceptionForIncompleteOutput(output, errs, scriptId);
				return (exitCode, output.Skip(1).Where(o => o != null && !o.Contains("dasmeta")).ToArray());
			}
			catch (Exception e)
			{
				throw new Exception(e.Message, e);
			}
		}

		/// <summary>
		/// start the process and collect std output until it dies or the timeout is run down
		/// </summary>
		/// <param name="psi">RedirectStandardOutput/Error set to true, Shell
		///     Execute=false, fully loaded arglist including /K to keep the command shell open</param>
		/// <param name="output">on entry an empty list
		///     on exit -
		///     all lines from cmd.exe stdout and stderr or empty list</param>
		/// <param name="suppressLog"></param>
		/// <returns>exit code from script (which is whatever the last executed step exits with
		///   or 1 if the args are wrong) or int.MaxValue - 1 to indicate timeout</returns>
		private int RunCmdProcess(ProcessStartInfo psi, List<string> output, bool suppressLog)
		{
			int exitCode;
			Process ps;
			using (ps = Process.Start(psi))
			{
				string s;
				do
				{
					s = ps.StandardOutput.ReadLine();
					if (s != null)
					{
						output.Add(s);
					}
				} while (s != null);

				var result = ps.WaitForExit(scriptTimeout);
				exitCode = result ? ps.ExitCode : int.MaxValue - 1;
						// I suspect result will always be true.  ReadLine should be blocked until the
						// process exits so there will be no wait period - still, who knows?
			}
			if (!suppressLog) logger.LogDebug($"exit code: {exitCode}");
			return exitCode;
		}

		private void ThrowExceptionForIncompleteOutput(List<string> output, List<string> errs, string scriptName)
		{
			output.Where(o => o != null).ToList().ForEach(o => logger.LogDebug(o));
			errs.Where(o => o != null).ToList().ForEach(o => logger.LogDebug(o));
			if (!(output.Any(o => o.StartsWith("dasmeta_output_complete")) || output.Any(o => o.StartsWith("dasmeta_errors_complete"))))
			{
				throw new Exception($"incomplete output from script {scriptName}");
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

		private string GetCmdExe(bool suppressLog)
		{
			return scriptPlatform.GetCmdExe(suppressLog);
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
