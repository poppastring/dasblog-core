using System;
using System.Diagnostics;
using System.IO;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;

namespace DasBlog.Tests.Support
{
	public class Alerter : IAlerter
	{
		/// <summary>
		/// It turns out the need for this Heath Robinson/Rube Goldberg mechanism was not critical
		/// Although xunit swallows exceptions (which was the motivation) it only does it for those that occur
		/// in the SUT.
		/// I will keep this around for the really urgent messages.
		/// TODO - this should be suppressed on Appveyor where it will probably not be needed and will cause trouble
		/// because the alert blocks the execution.
		/// </summary>
		/// <param name="message">text to be displayed to the user</param>
		/// <exception cref="Exception">if everything else turns bad - do our best and let the
		///   user have a stack trace to look at</exception>
		public void Alert(string message)
		{
			var comSpec = Environment.GetEnvironmentVariable("ComSpec");
			ProcessStartInfo psi = new ProcessStartInfo(comSpec);
			var alertUserScript = Path.Combine(Utils.GetProjectRootDirectory(),
				Constants.ScriptsRelativePath, "AlertUser.cmd");
			psi.UseShellExecute = true;
			psi.CreateNoWindow = false;		// I don't think it affects console apps.
			psi.Arguments =
				$@"/Q /U /K {alertUserScript} ""{message}""";
			var ps = Process.Start(psi);
			ps.WaitForExit();
			if (ps.ExitCode != 0)
			{
				throw new Exception($"{alertUserScript} script exited with non-zero exit code: {ps.ExitCode}");
			}
		}
	}
}
