using System;
using System.Collections;
using System.IO;
using newtelligence.DasBlog.Runtime;
using NUnit.Framework;
using System.Collections.Generic;
using NodaTime;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for EntryTests.
	/// </summary>
	[TestFixture]
	public class BlogDataServiceTests : TestBaseLocal
	{
		#region Tests
		DirectoryInfo createEntries;

		[SetUp]
		public void EntriesSetup()
		{
			createEntries = new DirectoryInfo(Path.Combine(this.testContent.FullName, "CreateEntries"));
			// delete old entries
			if (createEntries.Exists)
			{
				createEntries.Delete(true);
				createEntries.Create();
			}
			else
			{
				createEntries.Create();
			}
		}

		[Test]
		public void GetEntriesForDay()
		{
			IBlogDataService dataService = BlogDataServiceFactory.GetService(createEntries.FullName, null);
			// this will create an entry for each hour of the day in local time
			for (int i = 0; i < 24; i++)
			{
				Entry entry = TestEntry.CreateEntry(String.Format("Test Entry {0}", i), 5, 2);
				entry.CreatedUtc = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, i, 0, 0).ToUniversalTime();
				dataService.SaveEntry(entry, null);
			}

			EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now, DateTimeZone.Utc, String.Empty, int.MaxValue, int.MaxValue, String.Empty);

			Assert.AreEqual(24, entries.Count);

			foreach (Entry entry in entries)
			{
				// this test will make sure that the entries are all in the right day
				Entry lookup = dataService.GetEntry(entry.EntryId);
				Assert.IsNotNull(lookup);
				Assert.AreEqual(0, lookup.CompareTo(entry));
				Assert.AreEqual(DateTime.Now.Date, entry.CreatedLocalTime.Date);
			}
		}

		[Test]
		public void GetEntry()
		{
			IBlogDataService dataService = BlogDataServiceFactory.GetService(createEntries.FullName, null);
			
			Entry entry = TestEntry.CreateEntry(String.Format("Test Entry"), 5, 2);
			dataService.SaveEntry(entry, null);
		
			DateTime dt = DateTime.Now;
			Entry returnEntry = dataService.GetEntry(entry.EntryId);
			TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
			Console.WriteLine(ts.Milliseconds);
			Assert.AreEqual(0, entry.CompareTo(returnEntry));
		}

//		[Test]
//		public void GetEntryTitle()
//		{
//			IBlogDataService dataService = BlogDataServiceFactory.GetService(createEntries.FullName, null);
//			
//			Entry entry = TestEntry.CreateEntry(String.Format("Test Entry"), 5, 2);
//			dataService.SaveEntry(entry, null);
//		
//			DateTime dt = DateTime.Now;
//			string title = dataService.GetEntryTitle(entry.EntryId);
//			TimeSpan ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
//			Console.WriteLine(ts.Milliseconds);
//			Assert.AreEqual(entry.Title, title);
//		}

		// Ignoring this test: The comment mentions something about hours, but the test increments days.
		// Also, the logic seems messed up if the current day of the month is less than 12 days from the end of the month.
        [Test, Ignore("Test does not make sense.")]
        public void GetDaysWithEntries()
        {
            List<int> dayNumbers = new List<int>();
            IBlogDataService dataService = BlogDataServiceFactory.GetService(createEntries.FullName, null);
            // this will create an entry for each hour of the day in local time
            for (int i = 0; i < 24; i++)
            {
                Entry entry = TestEntry.CreateEntry(String.Format("Test Entry {0}", i), 5, 2);
                entry.CreatedUtc = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.AddDays(i).Day, 1, 0, 0).ToUniversalTime();
                dayNumbers.Add(DateTime.Now.AddDays(i).Day);
                i++;
                dataService.SaveEntry(entry, null);
            }
            
            DateTime[] days = dataService.GetDaysWithEntries(DateTimeZone.Utc);
            
            for (int i = 0; i < 12; i++)
            {
                int num = (int)dayNumbers[11 - i];
                Assert.AreEqual(num, days[i].Day);
                i++;
            }
        }
		
		[TearDown]
		public void EntriesDelete()
		{
			// delete old entries
			if (createEntries.Exists)
			{
				createEntries.Delete(true);
			}
		}
		#endregion Tests
	}
}
