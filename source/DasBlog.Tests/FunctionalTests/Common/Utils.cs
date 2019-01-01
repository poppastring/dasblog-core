using System;
using System.Globalization;
using System.IO;
using DasBlog.Managers.Interfaces;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.Common
{
	internal static partial class Utils
	{
		/// <summary>
		/// writes day entry file into the content directory 
		/// </summary>
		/// <param name="blogManager"></param>
		/// <param name="directory">e.g. c:/projects/dasblog-core.../Environments/Vanilla/content</param>
		/// <param name="entryId"></param>
		public static void SaveEntryDirect(string directory, string entryId = null)
		{
			var fileName = TestDataProcesor.GetBlogEntryFileName(DateTime.Today);
			var path = Path.Combine(directory, fileName);
			var str = string.Format(minimalBlogPostXml
				, DateTime.Today.ToString("s", CultureInfo.InvariantCulture)
				, DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
				, DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
				, entryId ?? Guid.NewGuid().ToString());
			File.WriteAllText(path, str);
//			new CacheFixer().InvalidateCache(blogManager);
		}

		public static void DeleteDirectoryContentsDirect(string directory, string fileSpec = "*.*")
		{
			foreach (var file in Directory.EnumerateFiles(directory, fileSpec))
			{
				File.Delete(file);
			}
		}
		public static bool DayEntryFileExists(string directory, DateTime dt)
		{
			var fileName = TestDataProcesor.GetBlogEntryFileName(dt);
			var path = Path.Combine(directory, fileName);
			return File.Exists(path);
		}
		public static bool DayFeedbackFileExists(string directory, DateTime dt)
		{
			var fileName = TestDataProcesor.GetBlogFeedbackFileName(dt);
			var path = Path.Combine(directory, fileName);
			return File.Exists(path);
		}
		
		public static Entry MakeMiniimalEntry()
		{
			Entry entry = new Entry();
			entry.Initialize();
			entry.Title = string.Empty;
			entry.Content = string.Empty;
			entry.Description = string.Empty;
			entry.Categories = string.Empty;
			return entry;
		}
	}
}
