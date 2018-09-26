using System.Collections.Generic;

namespace DasBlog.Tests.Support.Interfaces
{
	public interface IScriptRunner
	{
		/// <summary>
		/// runs a script and returns the exit code, anything written to stdout and stderr to the caller as lists of lines
		/// </summary>
		/// <param name="scriptName">no path, must include file extension e.g. DetectChanges.cmd</param>
		/// <param name="envirnmentVariables">any additional env vars required for the script
		///   which has access to this process's environment as well</param>
		/// <param name="agruments">command line arguments</param>
		/// <returns>standard cmd exit code i.e. 0=success, non-zero failure, a list of lines
		///    written to stdout and a list of lines written to stderr
		///   int.MaxValue indicates that the script was not run
		///   The last element in output and errors appears to be a null</returns>
		(int exitCode, string[] output, string[] errors) Run(
			string scriptName, IReadOnlyDictionary<string, string> envirnmentVariables, params object[] agruments);
	}
}
