using System;
using System.Collections.Generic;
using System.IO;

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
			string pathAndFile = Path.Combine(path, $"logs-{date.ToString("yyyyMMdd")}.txt");
			if (File.Exists(pathAndFile))
			{
				stream = new FileStream(pathAndFile, FileMode.Open, FileAccess.Read, FileShare.Read);
				StreamReader tr = new StreamReader(stream);
				string line;
				while ((line = tr.ReadLine()) != null)
				{
					yield return line;
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
