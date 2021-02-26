using System;
using System.IO;
using System.Linq;

namespace DasBlog.CLI
{
	public static class InitializeConfigFiles
	{
		public static void CopyFiles(string source, string environment)
		{
			if (string.IsNullOrWhiteSpace(environment))
			{
				environment = "Production";
			}

;			var dir = new DirectoryInfo(source);
			var files = dir.GetFiles()
					.Where(s => !s.Name.Contains(".Development."))
					.Where(s => !s.Name.Contains(".Production."))
					.Where(s => !s.Name.Contains(".Staging.")).ToArray();

			foreach (var file in files)
			{
				var renamefile = file.Name.Replace(".", $".{environment}.");

				if (!File.Exists(Path.Combine(dir.FullName, renamefile)))
				{
					file.CopyTo(Path.Combine(dir.FullName, renamefile), false);
				}
			}
		}

		public static bool IsInitialized(string source, string environment)
		{
			environment = SubstituteEnvironment(environment);

			return (new DirectoryInfo(source).GetFiles($"*.{environment}.*").Length == 4);
		}

		private static string SubstituteEnvironment(string environment)
		{
			if (string.IsNullOrWhiteSpace(environment))
			{
				environment = "Production";
			}

			return environment;
		}
	}
}
