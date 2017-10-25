using newtelligence.DasBlog.Util;

using NUnit.Framework;

using System;
using System.IO;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime.Test
{
	[TestFixture]
	public class EntryCollectionTest : BlogServiceContainer
	{
		[Test]
		public void VerifyContains()
		{
			Entry entry1, entry2;
			entry1 = BlogDataService.GetEntry("ebd3060-b3b0-4ab1-baea-cd12ae34a575");
			EntryCollection entries = new EntryCollection();
			entries.Add(entry1);
			Assert.IsTrue(entries.Contains(entry1), "Unexpectedly the entry did not exist.");
			entry2 = BlogDataService.GetEntry("ebd3060-b3b0-4ab1-baea-cd12ae34a575");
			// The entries returned are identical (ReferenceEquals)
			Assert.AreSame(entry1, entry2, "Unexpectedly and entirely different entry instance was returned.");
			Assert.IsTrue(entries.Contains(entry2), "Unexpectedly the entry did not exist.");
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

	}
}
















