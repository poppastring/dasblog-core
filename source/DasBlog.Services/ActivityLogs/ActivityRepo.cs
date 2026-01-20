using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DasBlog.Services.ActivityLogs
{
	public class ActivityRepo : IActivityRepo
	{
		private readonly string path;
		private FileStream stream;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="path">e.g. c:\\myapp\logs"</param>
		public ActivityRepo(string path)
		{
			System.Diagnostics.Debug.Assert(path != null);
			this.path = path;
		}
		public void Dispose()
		{
			stream?.Dispose();
		}

		public IEnumerable<string> GetEventLines(DateTime date)
		{
			// Try multiple filename patterns that the rolling file logger might use
			string baseFileName = $"logs-{date:yyyyMMdd}";
			string[] possibleFiles = new[]
			{
				Path.Combine(path, $"{baseFileName}.txt"),      // logs-20260119.txt
				Path.Combine(path, $"{baseFileName}.0.txt"),    // logs-20260119.0.txt (rolled file)
				Path.Combine(path, "logs.txt")                   // logs.txt (current file)
			};

			string pathAndFile = possibleFiles.FirstOrDefault(f => File.Exists(f));

			if (pathAndFile != null && File.Exists(pathAndFile))
			{
				stream = new FileStream(pathAndFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
				StreamReader tr = new StreamReader(stream);
				string line;
				while ((line = tr.ReadLine()) != null)
				{
					// Filter lines to only include those from the requested date
					if (line.StartsWith(date.ToString("yyyy-MM-dd")))
					{
						yield return line;
					}
				}
				tr.Close();
			}
		}
	}

	public class ActivityRepoOptions
	{
		public string Path { get; set; }
	}
}
