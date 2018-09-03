using System;
using System.Collections.Generic;
using System.Text;
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

			blogdataservice.DeleteEntry(entry.EntryId, null);

			var entryFailRetrieval = blogdataservice.GetEntry(entry.EntryId);
			Assert.Null(entryFailRetrieval);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetEntryForEditDelete_Successful(IBlogDataService blogdataservice)
		{
			var entry = TestEntry.CreateEntry(String.Format("Test Entry"), 5, 2);
			blogdataservice.SaveEntry(entry, null);

			var dt = DateTime.Now;
			var returnEntry = blogdataservice.GetEntryForEdit(entry.EntryId);
			Assert.NotNull(returnEntry);

			blogdataservice.DeleteEntry(entry.EntryId, null);

			var entryFailRetrieval = blogdataservice.GetEntry(entry.EntryId);
			Assert.Null(entryFailRetrieval);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_UpdateEntriesDelete_Successful(IBlogDataService blogdataservice)
		{
			var entry = TestEntry.CreateEntry(String.Format("Test Entry"), 5, 2);
			entry.Categories = "test";
			blogdataservice.SaveEntry(entry, null);

			var dt = DateTime.Now;
			var returnEntry = blogdataservice.GetEntryForEdit(entry.EntryId);
			Assert.NotNull(returnEntry);

			returnEntry.Title = "Test Entry 2";
			var state = blogdataservice.SaveEntry(returnEntry, null);
			Assert.True(state == EntrySaveState.Updated);

			var entryAfterUpdate = blogdataservice.GetEntry(entry.EntryId);
			Assert.True(entryAfterUpdate.Title == "Test Entry 2");

			blogdataservice.DeleteEntry(entry.EntryId, null);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_GetEntriesForDay_Successful(IBlogDataService blogdataservice)
		{
			var entries = blogdataservice.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, string.Empty, int.MaxValue, int.MaxValue, string.Empty);
			Assert.NotNull(entries);
		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		[Trait("Category", "UnitTest")]
		public void BlogDataService_SearchEntries_Successful(IBlogDataService blogdataservice)
		{
			var entries = blogdataservice.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, string.Empty, int.MaxValue, int.MaxValue, string.Empty);
			Assert.NotNull(entries);
		}

		public static TheoryData<IBlogDataService> DasBlogDataService = new TheoryData<IBlogDataService>
		{
			BlogDataServiceFactory.GetService(UnitTestsConstants.TestContentLocation, LoggingDataServiceFactory.GetService(UnitTestsConstants.TestLoggingLocation))
		};
	}
}
