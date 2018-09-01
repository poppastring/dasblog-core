using System;
using System.Collections.Generic;
using System.Text;
using newtelligence.DasBlog.Runtime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
    public class EntryCollectionTest
    {
		[Fact]
		[Trait("Category", "UnitTest")]
		public void EntryCollections_FilteringPublicEntries_CountsShouldDifferAfterFiltering()
		{
			var entry1 = CreateEntry("public 2007", true, new DateTime(2007, 2, 2));
			var entry2 = CreateEntry("public 2006", true, new DateTime(2006, 2, 2));
			var entry3 = CreateEntry("not public 2006", false, new DateTime(2006, 2, 2));

			var collection = new EntryCollection(new Entry[] { entry1, entry2, entry3 });

			Predicate<Entry> filter = null;

			Predicate<Entry> isPublic = delegate (Entry e)
			{
				return e.IsPublic;
			};

			filter += isPublic;

			Predicate<Entry> is2006Published = delegate (Entry e)
			{
				return e.CreatedLocalTime.Year == 2006;
			};

			filter += is2006Published;

			var classic = EntryCollectionFilter.FindAll(collection, filter);
			var generic = collection.FindAll(filter);

			Assert.NotEqual(classic.Count, generic.Count);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void EntryCollections_FilterAndMaxResults_OnlyReturnsOneEntry()
		{
			var entry1 = CreateEntry("public 2007.1", true, new DateTime(2007, 2, 2));
			var entry2 = CreateEntry("public 2006.1", true, new DateTime(2006, 2, 2));
			var entry3 = CreateEntry("not public 2006.1", false, new DateTime(2006, 2, 2));
			var entry4 = CreateEntry("public 2007.2", true, new DateTime(2007, 2, 2));
			var entry5 = CreateEntry("public 2006.2", true, new DateTime(2006, 2, 2));
			var entry6 = CreateEntry("not public 2006.2", false, new DateTime(2006, 2, 2));

			var collection = new EntryCollection(new Entry[] { entry1, entry2, entry3, entry4, entry5, entry6 });

			Predicate<Entry> filter = null;

			Predicate<Entry> isPublic = delegate (Entry e)
			{
				return e.IsPublic;
			};

			filter += isPublic;

			Predicate<Entry> is2006Published = delegate (Entry e)
			{
				return e.CreatedLocalTime.Year == 2006;
			};

			filter += is2006Published;

			// 2 items match the criteria (2,5), we only want 1 returned
			var filtered = EntryCollectionFilter.FindAll(collection, filter, 1);
			Assert.Single(filtered);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void EntryCollections_NoFilterMaxResults_OnlyReturnsFourEntries()
		{
			var entry1 = CreateEntry("public 2007.1", true, new DateTime(2007, 2, 2));
			var entry2 = CreateEntry("public 2006.1", true, new DateTime(2006, 2, 2));
			var entry3 = CreateEntry("not public 2006.1", false, new DateTime(2006, 2, 2));
			var entry4 = CreateEntry("public 2007.2", true, new DateTime(2007, 2, 2));
			var entry5 = CreateEntry("public 2006.2", true, new DateTime(2006, 2, 2));
			var entry6 = CreateEntry("not public 2006.2", false, new DateTime(2006, 2, 2));

			var collection = new EntryCollection(new Entry[] { entry1, entry2, entry3, entry4, entry5, entry6 });

			// no predicate so all results should be returend but we only want 4
			var maxResults = EntryCollectionFilter.FindAll(collection, null, 4);
			Assert.Equal(4, maxResults.Count);
		}

		private static Entry CreateEntry(string title, bool isPublic, DateTime publishData)
		{
			var entry = new Entry();
			entry.Initialize();
			entry.Title = title;
			entry.IsPublic = isPublic;
			entry.CreatedLocalTime = publishData;

			return entry;
		}
	}
}
