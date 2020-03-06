using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DasBlog.CLI
{
	public static class InitializeConfigFiles
	{
		public static void CopyFiles(string source, string environment)
		{
;			var dir = new DirectoryInfo(source);
			var files = dir.GetFiles("*.Development.*");

			foreach (var file in files)
			{
				var renamefile = file.Name.Replace("Development", environment);
				
				file.CopyTo(Path.Combine(dir.FullName, renamefile), false);
			}
		}

		public static bool IsInitialized(string source, string environment)
		{
			return (new DirectoryInfo(source).GetFiles($"*.{environment}.*").Length == 4);
		}
	}
}
