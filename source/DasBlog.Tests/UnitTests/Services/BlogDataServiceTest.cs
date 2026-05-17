using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class BlogDataServiceTest
	{
		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_CreateGetDeleteEntry_Successful(IBlogDataService blogdataservice)
		{
			var entry = TestEntry.CreateEntry(String.Format("Test Entry"), 5, 2);
			blogdataservice.SaveEntry(entry, null);

			var dt = DateTime.Now;
			var returnEntry = blogdataservice.GetEntry(entry.EntryId);
			Assert.Equal(0, entry.CompareTo(returnEntry));

			blogdataservice.DeleteEntry(entry.EntryId);

			var entryFailRetrieval = blogdataservice.GetEntry(entry.EntryId);
			Assert.Null(entryFailRetrieval);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetEntryForEditDelete_Successful(IBlogDataService blogdataservice)
		{
			var entriesBefore = blogdataservice.GetEntries(false);
			var entry = TestEntry.CreateEntry(String.Format("Test Entry 2"), 5, 2);
			blogdataservice.SaveEntry(entry, null);

			var dt = DateTime.Now;
			var returnEntry = blogdataservice.GetEntryForEdit(entry.EntryId);
			Assert.NotNull(returnEntry);

			blogdataservice.DeleteEntry(entry.EntryId);

			var entryFailRetrieval = blogdataservice.GetEntry(entry.EntryId);
			Assert.Null(entryFailRetrieval);
			var entriesAfter = blogdataservice.GetEntries(false);
			Assert.Equal(entriesBefore, entriesAfter);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_UpdateEntriesDelete_Successful(IBlogDataService blogdataservice)
		{
			var entry = TestEntry.CreateEntry(String.Format("Test Entry 3"), 5, 2);
			entry.Categories = "test";
			blogdataservice.SaveEntry(entry, null);

			var dt = DateTime.Now;
			var returnEntry = blogdataservice.GetEntryForEdit(entry.EntryId);
			Assert.NotNull(returnEntry);

			returnEntry.Title = "Test Entry 4";
			var state = blogdataservice.SaveEntry(returnEntry, null);
			Assert.True(state == EntrySaveState.Updated);

			var entryAfterUpdate = blogdataservice.GetEntry(entry.EntryId);
			Assert.True(entryAfterUpdate.Title == "Test Entry 4");

			blogdataservice.DeleteEntry(entry.EntryId);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_HeroImageRoundTrip_Successful(IBlogDataService blogdataservice)
		{
			var entry = TestEntry.CreateEntry("Test Hero Image Entry", 5, 2);
			entry.ImageUrl = "https://cdn.example.com/hero.jpg";
			entry.ImageAlt = "Hero alt text";
			blogdataservice.SaveEntry(entry, null);

			var saved = blogdataservice.GetEntry(entry.EntryId);
			Assert.NotNull(saved);
			Assert.Equal("https://cdn.example.com/hero.jpg", saved.ImageUrl);
			Assert.Equal("Hero alt text", saved.ImageAlt);

			// Update the hero image fields and verify the change is persisted.
			var forEdit = blogdataservice.GetEntryForEdit(entry.EntryId);
			forEdit.ImageUrl = "https://cdn.example.com/hero2.jpg";
			forEdit.ImageAlt = "Updated alt";
			var state = blogdataservice.SaveEntry(forEdit, null);
			Assert.True(state == EntrySaveState.Updated);

			var updated = blogdataservice.GetEntry(entry.EntryId);
			Assert.Equal("https://cdn.example.com/hero2.jpg", updated.ImageUrl);
			Assert.Equal("Updated alt", updated.ImageAlt);

			// Clearing hero image fields should also round-trip.
			var forClear = blogdataservice.GetEntryForEdit(entry.EntryId);
			forClear.ImageUrl = string.Empty;
			forClear.ImageAlt = string.Empty;
			blogdataservice.SaveEntry(forClear, null);

			var cleared = blogdataservice.GetEntry(entry.EntryId);
			Assert.True(string.IsNullOrEmpty(cleared.ImageUrl));
			Assert.True(string.IsNullOrEmpty(cleared.ImageAlt));

			blogdataservice.DeleteEntry(entry.EntryId);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetPublicEntriesForDayMaxValue_Successful(IBlogDataService blogdataservice)
		{
			var entries = blogdataservice.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, string.Empty, int.MaxValue, int.MaxValue, string.Empty);
			Assert.Equal(19, entries.Count);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_SearchEntries_Successful(IBlogDataService blogdataservice)
		{
			////
			var entries = blogdataservice.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, string.Empty, int.MaxValue, int.MaxValue, string.Empty);
			Assert.NotNull(entries);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_ArchiveMonthSearch_Successful(IBlogDataService blogdataservice)
		{
			var dt = new DateTime(2004, 3, 1);
			var entries = blogdataservice.GetEntriesForMonth(dt, DateTimeZone.Utc, "");
			Assert.Equal(5, entries.Count);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetDaysWithEntries_Successful(IBlogDataService blogdataservice)
		{
			var dates = blogdataservice.GetDaysWithEntries(DateTimeZone.Utc).ToList<DateTime>();
			Assert.Equal(10, dates.Count);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetEntriesCategory_Successful(IBlogDataService blogdataservice)
		{
			var entries = blogdataservice.GetEntriesForCategory("A Random Mathematical Quotation", "");
			Assert.Equal(7, entries.Count);
		}
		[Fact]
//		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetEntriesWithFalse_Successful()
		{
			BlogDataServiceFactory.RemoveService(UnitTestsConstants.TestContentLocation);				
			IBlogDataService blogdataservice = BlogDataServiceFactory.GetService(UnitTestsConstants.TestContentLocation,
			  LoggingDataServiceFactory.GetService(UnitTestsConstants.TestLoggingLocation));
			// gets both public and non-public
			var entries = blogdataservice.GetEntries(false);
			Assert.Equal(23, entries.Count);
		}

		public static TheoryData<IBlogDataService> DasBlogDataService = new TheoryData<IBlogDataService>
		{
			BlogDataServiceFactory.GetService(UnitTestsConstants.TestContentLocation, LoggingDataServiceFactory.GetService(UnitTestsConstants.TestLoggingLocation))
		};
	}
}
