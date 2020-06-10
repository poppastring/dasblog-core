using System;
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

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Entry_CompareEntry_CompareDescriptionAndTitle()
		{
			var entry1 = TestEntry.CreateEntry("Test Entry", 5, 2);
			var entry2 = TestEntry.CreateEntry("Test Entry", 5, 2);

			// check to see if they contain the same simple types
			Assert.True(entry1.CompareTo(entry1) == 0);
			Assert.True(entry1.CompareTo(entry2) == 0);

			// change a simple type
			entry2.Title = "Test Entry 2";
			Assert.True(entry1.CompareTo(entry2) == 1, "entry1 is equal to entry2");

			entry2.Description = "Some Description";
			Assert.True(entry1.CompareTo(entry2) == 1);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Entry_CompareEntry_CloneCompare()
		{
			var entry = new Entry();
			entry.Initialize();

			var same = entry;
			Assert.Same(entry, same);

			var copy = entry.Clone();
			bool equals = entry.Equals(copy);
			Assert.True(!equals);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Entry_CompressTitle_ConfirmItMatches()
		{
			TitleTest(@" This    is a  test ", "this-is-a-test");
			TitleTest(@" - _ . ! * ' ( ) ", "");
			TitleTest(@"So <em>Not</em> true", "so-not-true");
			TitleTest(@"Three is < four", "three-is-four");
			TitleTest(@"Three is < four but > one", "three-is-one");
			TitleTest(@"My <sarcasm>favorite</sarcasm> bug", "my-favorite-bug");
			TitleTest(@"What you’re seeing", "what-youre-seeing");

			//TODO: Figure out why this test fails...
			// TitleTest("\u00C0\u00C3\u00C5\u00c6\u00c8 \u00CC\u00CD\u00D0 \u0105\u0157\u0416\u042D \u0628\u0645\u1E84\uFB73",
			//		"%c3%80%c3%83%c3%85%c3%86%c3%88+%c3%8c%c3%8d%c3%90+%c4%84%c5%97%d0%96%d0%ad+%d8%a8%d9%85%e1%ba%84%ef%ad%b3");
		}

		private void TitleTest(string title, string expected)
		{
			var result = Entry.InternalCompressTitle(title, "-").ToLower();
			Assert.Equal(expected, result);
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
