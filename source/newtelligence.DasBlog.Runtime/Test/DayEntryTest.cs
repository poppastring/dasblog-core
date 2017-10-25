using newtelligence.DasBlog.Util;

using NUnit.Framework;

using System;
using System.IO;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime.Test
{
	[TestFixture]
	public class DayEntryTest : BlogServiceContainer
	{
		[Test]
		public void LoadDayEntry()
		{
			IDayEntry dayEntry = BlogDataService.GetDayEntry(new DateTime(2004, 3, 1));
			Assert.AreEqual(2, dayEntry.GetEntries().Count);
		}

		[Test]
		public void SerializeNewEntry()
		{
			FileStream fileStream = null;

			Entry entry = new Entry();
			entry.Initialize();
			entry.Title = "The Title";
			entry.Content = "Content";
			entry.EntryId = Guid.NewGuid().ToString();
			
			DayEntry dayEntry = new DayEntry();
			dayEntry.Initialize();
			dayEntry.Entries.Add(entry);
			try
			{
				fileStream = FileUtils.OpenForWrite(
					Path.Combine(CONTENT_DIRECTORY_PATH, dayEntry.FileName));
				if(fileStream == null)
				{
					Assert.Fail("Unable to create stream {0}",
						Path.Combine(CONTENT_DIRECTORY_PATH, dayEntry.FileName));
				}
				else
				{
					XmlSerializer ser = new XmlSerializer(typeof(DayEntry), new Type[]{typeof(Entry)});
					using (StreamWriter writer = new StreamWriter(fileStream))
					{
						ser.Serialize(writer, dayEntry);
					}
					using(StreamReader reader = new StreamReader(Path.Combine(CONTENT_DIRECTORY_PATH, dayEntry.FileName)))
					{																															  
						Console.WriteLine(reader.ReadToEnd());
					}
					
				}
			}
			finally
			{
				if(fileStream != null)
				{
					fileStream.Close();
				}
			}
		}


		/* TODO:  No longer possible because using interface.
		[Test]
		public void ModifyAndSerializeExistingEntry()
		{
			FileStream fileStream = null;
			IDayEntry dayEntry = BlogDataService.GetDayEntry(new DateTime(2004, 3, 01));
			dayEntry.DateUtc = DateTime.Now;
			try
			{
				fileStream = FileUtils.OpenForWrite(
					Path.Combine(CONTENT_DIRECTORY_PATH, dayEntry.FileName));
				if(fileStream == null)
				{
					Assert.Fail("Unable to create stream {0}",
						Path.Combine(CONTENT_DIRECTORY_PATH, dayEntry.FileName));
				}
				else
				{
					XmlSerializer ser = new XmlSerializer(typeof(DayEntry));
					using (StreamWriter writer = new StreamWriter(fileStream))
					{
						ser.Serialize(writer, dayEntry);
					}
				}
			}
			finally
			{
				if(fileStream != null)
				{
					fileStream.Close();
				}
			}
		}
		*/

	}
}
















