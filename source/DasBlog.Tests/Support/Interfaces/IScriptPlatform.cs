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
	}
}
