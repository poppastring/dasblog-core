using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using NUnit.Framework;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading;

namespace newtelligence.DasBlog.Runtime.Test
{
	[TestFixture]
	public class BlogDataServiceTest : BlogServiceContainer
	{
		[Test]
		public void VerifySaveEntry()
		{
			Guid entryId = Guid.NewGuid();
			Entry entry = new Entry();
			entry.Initialize();
			entry.CreatedUtc = new DateTime(2004, 1, 1);
			entry.Title = "Happy New ear";
			entry.Content = "The beginning of a new year.";
			entry.Categories = "Test";
			entry.EntryId = entryId.ToString();
			entry.Description = "The description of the entry.";
			entry.Author = "Inigo Montoya";
			// TODO:  Update so private entries will work as well.
			entry.IsPublic = true;
			entry.Language = "C#";
			entry.Link = "http://mark.michaelis.net/blog";
			entry.ModifiedUtc = entry.CreatedUtc;
			entry.ShowOnFrontPage = false;
			BlogDataService.SaveEntry( entry );

			entry = BlogDataService.GetEntry(entryId.ToString());
			Assert.IsNotNull(entry, "Unable to retrieve the specified entry.");
			Assert.AreEqual(entryId.ToString(), entry.EntryId);
			Assert.AreEqual(new DateTime(2004, 1, 1), entry.CreatedUtc);
			Assert.AreEqual("Happy New ear", entry.Title);
			Assert.AreEqual("The beginning of a new year.", entry.Content);
			Assert.AreEqual("Test", entry.Categories);
			Assert.AreEqual("The description of the entry.", entry.Description);
			Assert.AreEqual("Inigo Montoya", entry.Author);
			Assert.AreEqual(true, entry.IsPublic);
			Assert.AreEqual("C#", entry.Language);
			Assert.AreEqual("http://mark.michaelis.net/blog", entry.Link);
			Assert.AreEqual(entry.CreatedUtc, entry.ModifiedUtc);
			Assert.AreEqual(false, entry.ShowOnFrontPage);
		}


		[Test]
		public void GetDayEntry()
		{
			IDayEntry dayEntry;
			
			// Note that non public items are ignored.
			dayEntry = BlogDataService.GetDayEntry(new DateTime(2003, 7, 31));
			Assert.AreEqual(6, dayEntry.GetEntries().Count);

			// Note that non public items are ignored.
			dayEntry = BlogDataService.GetDayEntry(new DateTime(2003, 8, 1));
			Assert.AreEqual(4, dayEntry.GetEntries().Count);
		}

		[Test]
		public void VerifyGetEntriesFormMonth()
		{
			EntryCollection entries;
			TimeZone timeZone = TimeZone.CurrentTimeZone;
			DateTime dateTime;

			// Since TimeZone does not have a constructor for a particular
			// time zone we only run this test for the Pacific Time Zone.
			if(timeZone.StandardName == "Pacific Standard Time")
			{
				dateTime = new DateTime(2004, 3, 4).ToUniversalTime();

				entries = BlogDataService.GetEntriesForMonth(dateTime, 
					timeZone, null);
				Assert.AreEqual(3, entries.Count);

				// Note that non-public items are not returned.
				dateTime = new DateTime(2003, 7, 31).ToUniversalTime();
				entries = BlogDataService.GetEntriesForMonth(dateTime, 
					timeZone, null);
				Assert.AreEqual(8, entries.Count);

				dateTime = new DateTime(2003, 1, 4).ToUniversalTime();
				entries = BlogDataService.GetEntriesForMonth(dateTime, 
					timeZone, null);
				Assert.AreEqual(0, entries.Count);
			}
		}

		[Test]
		public void VerifyGetEntriesForCategory()
		{
			EntryCollection entries;

			// Note that non-public entries are not returned by default.
			entries = BlogDataService.GetEntriesForCategory(
				"G. K. Chesterton Quotes", null);
			Assert.AreEqual(10, entries.Count);

			entries = BlogDataService.GetEntriesForCategory(
				"G. K. Chesterton Quotes|More By Chesterton", null);
			Assert.AreEqual(2, entries.Count);

			// Note that non-public entries are not returned by default.
			entries = BlogDataService.GetEntriesForCategory(
				"G. K. Chesterton Quotes|Private Entry", null);
			Assert.AreEqual(0, entries.Count);

		}

		[Test]
		public void VerifyGetEntry()
		{
			Entry entry;
			IPrincipal originalPrincipal;

			entry = BlogDataService.GetEntry(
				"0c805dc8-e389-4d05-9bd8-9a99a28d78f4");
			Assert.AreEqual("Interesting but not necessarily true.", entry.Title);

			// Don't return private entries by default.
			entry = BlogDataService.GetEntry(
				"40f5dbc6-3a50-4bea-833b-8be993886258");
			Assert.IsNull(entry, "Unexpectedly we retrieved a private entry.");

			originalPrincipal = Thread.CurrentPrincipal;
			// Return private entries if requested explicitly.
			Thread.CurrentPrincipal = new GenericPrincipal(
				new GenericIdentity("junk", "junk"), new string[]{"admin"});
			entry = BlogDataService.GetEntry(
				"40f5dbc6-3a50-4bea-833b-8be993886258");
			Assert.AreEqual("Compatible Transitional Capability (CTC)", entry.Title);

			Thread.CurrentPrincipal = originalPrincipal;
			// Don't return private entries by default.
			entry = BlogDataService.GetEntry(
				"40f5dbc6-3a50-4bea-833b-8be993886258");
			Assert.IsNull(entry, "Unexpectedly we retrieved a private entry.");

		}		

		[Test]
		public void VerifyGetDaysWithEntries()
		{
			TimeZone timeZone = TimeZone.CurrentTimeZone;
			DateTime[] daysWithEntries;

			// Since TimeZone does not have a constructor for a particular
			// time zone we only run this test for the Pacific Time Zone.
			if(timeZone.StandardName == "Pacific Standard Time")
			{
				daysWithEntries = BlogDataService.GetDaysWithEntries(
					timeZone);
				Array.Sort(daysWithEntries);

				// Rather than checking the count we look for
				// particular items so that it does not cause 
				// problems when adding more test data.
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 3, 26)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 3, 14)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 3, 2)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 2, 29)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 2, 18)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 1, 25)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2004, 1, 12)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2003, 8, 1)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2003, 7, 31)) >= 0);
				Assert.IsTrue(Array.BinarySearch(daysWithEntries, new DateTime(2003, 7, 30)) >= 0);
			}
		}

		
		[Test]
		public void VerifyGetCategories()
		{
			string[] categories;
			bool GKChestertonQuotes = false;
			bool MoreByChesterton = false;
			bool ARandomMathematicalQuotation = false;
			bool Miscellaneous = false;
			bool PrivateEntry = false;

			categories = new string[BlogDataService.GetCategories().Count];
			for(int count=0; count<categories.Length; count++)
			{
				if(BlogDataService.GetCategories()[count].DisplayName == "G. K. Chesterton Quotes")
				{
					GKChestertonQuotes = true;
				}
				else if(BlogDataService.GetCategories()[count].DisplayName == "More By Chesterton")
				{
					MoreByChesterton = true;
				}
				else if(BlogDataService.GetCategories()[count].DisplayName == "A Random Mathematical Quotation")
				{
					ARandomMathematicalQuotation = true;
				}
				else if(BlogDataService.GetCategories()[count].DisplayName == "Miscellaneous")
				{
					Miscellaneous = true;
				}
				else if(BlogDataService.GetCategories()[count].DisplayName == "Private Entry")
				{
					PrivateEntry = true;
				}
			}

			Assert.IsTrue(GKChestertonQuotes);
			Assert.IsTrue(MoreByChesterton);
			Assert.IsTrue(ARandomMathematicalQuotation);
			Assert.IsTrue(Miscellaneous);
			Assert.IsFalse(PrivateEntry);

		}	
	
		[Test]
		public void VerifyPublicAreNotRetrievedByDefault()
		{
			IDayEntry dayEntry;
			
			dayEntry = BlogDataService.GetDayEntry(new DateTime(2003, 7, 31));
			Assert.AreEqual(6, dayEntry.GetEntries().Count,
				"The correct item count was not returned considering some items were private.");

			dayEntry = BlogDataService.GetDayEntry(new DateTime(2003, 8, 1));
			Assert.AreEqual(4, dayEntry.GetEntries().Count, 
				"The correct item count was not returned considering some items were private.");

		}
		
		// [Test]
		public void CreateTestEntries()
		{
			Entry entry;
			DateTime dateTime = new DateTime(2004, 01, 1);

			dateTime = dateTime.AddDays(12.33);
			entry = new Entry();
			entry.Initialize();
			entry.Title="Naivete of the Mathematician ";
			entry.Content="Mathematics is not yet capable of coping with the naivete of the mathematician himself.  (Sociology Learns the Language of Mathematics.)";
			entry.EntryId = Guid.NewGuid().ToString();
			entry.Author = "Abraham Kaplan";
			entry.CreatedUtc = dateTime;
			entry.ModifiedUtc = dateTime;
			entry.Categories = "A Random Mathematical Quotation";
			BlogDataService.SaveEntry(entry);
		}

	}
}
