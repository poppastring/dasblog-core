using System.Collections.ObjectModel;

namespace DasBlog.Tests.Support.Interfaces
{
	public interface IScriptPlatform
	{
		/// <summary>
		/// for windows this is the value of Comspec
		/// for posix this is bash
		/// </summary>
		/// <param name="suppressLog">when outside the scope of an xunit test logging causes
		///   an exception to be thrown - specifying true here prevents that</param>
		/// <returns>typically cmd.exe or bash without a path</returns>
		string GetCmdExe(bool suppressLog = false);

		/// <summary>
		/// subdirectory is either "cmd" or "bash"
		/// </summary>
		/// <param name="scriptId">typically the name of the script without a suffix</param>
		/// <returns>the subdirectory of scripts+name of script e.t. cmd/ConfirmStash.cmd</returns>
		string GetNameAndScriptSubDirectory(string scriptId);
		/// <summary>
		/// flags used by the shell itself rather than the script
		/// </summary>
		/// <returns>"/K" for windows "-c" for bash</returns>
		string[] GetShellFlags();

		/// <summary>
		/// bash needs the script name and its args as a single parameter whereas cmd.exe is happy for all args to be
		/// separate parameters.
		/// </summary>
		/// <param name="psiArgumentList">ProcessInfo.ArgumentList for the process that will execute the script</param>
		/// <param name="shellFlags">"/K" for windows, "-c" for bash</param>
		/// <param name="scriptPathAndName">e.g. /projects/dasblog-core/source/DasBlog.Tests/Support/Scripts/bash/DetectChanes.sh</param>
		/// <param name="scriptArgs">path location or guid or stash-hash</param>
		void GatherArgsForPsi(Collection<string> psiArgumentList
			, object[] shellFlags, string scriptPathAndName, object[] scriptArgs);
	}
}
