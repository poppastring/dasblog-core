using System;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using newtelligence.DasBlog.Runtime;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public partial class BlogManagerTests : IClassFixture<ComponentTestPlatform>, IDisposable
	{
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void Post_ForFirstInDay_CreatesFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				var entry = MakeMiniimalEntry();
				blogManager.CreateEntry(entry);
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				var savedEntryId = testDataProcessor.GetBlogPostValue(DateTime.Today, entry.EntryId, "EntryId");
				Assert.Equal(entry.EntryId.ToString(), savedEntryId.value);
			}
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void Post_ForSEcondInDay_CreatesFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				Entry firstEntry = MakeMiniimalEntry();
				blogManager.CreateEntry(firstEntry);
				Entry secondEntry = MakeMiniimalEntry();
				blogManager.CreateEntry(secondEntry);
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				var savedFirstEntryId = testDataProcessor.GetBlogPostValue(DateTime.Today, firstEntry.EntryId, "EntryId");
				Assert.Equal(firstEntry.EntryId.ToString(), savedFirstEntryId.value);
				var savedSecondEntryId = testDataProcessor.GetBlogPostValue(DateTime.Today, secondEntry.EntryId, "EntryId");
				Assert.Equal(secondEntry.EntryId.ToString(), savedSecondEntryId.value);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void Post_ForSEcondInDayWithoutCache_AddsEntryToFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				SaveEntryDirect(Path.Combine(sandbox.TestEnvironmentPath, Constants.ContentDirectory));
						// create the file and seed with an uncached post
				var blogManager = platform.CreateBlogManager(sandbox);
				var entry = MakeMiniimalEntry();
				blogManager.CreateEntry(entry);
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				var savedEntryId = testDataProcessor.GetBlogPostValue(DateTime.Today, entry.EntryId, "EntryId");
				Assert.Equal(entry.EntryId.ToString(), savedEntryId.value);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void Update_ExistngPost_ModifiesFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				Entry firstEntry = MakeMiniimalEntry();
				firstEntry.Title = "created";
				blogManager.CreateEntry(firstEntry);
				var savedFirstTitle = testDataProcessor.GetBlogPostValue(DateTime.Today, firstEntry.EntryId, "Title");
				Assert.Equal("created", savedFirstTitle.value);
				Entry secondEntry = MakeMiniimalEntry();
				secondEntry.EntryId = firstEntry.EntryId;
				secondEntry.Title = "updated";
				blogManager.UpdateEntry(secondEntry);
				var savedSecondTitle = testDataProcessor.GetBlogPostValue(DateTime.Today, secondEntry.EntryId, "Title");
				Assert.Equal("updated", savedSecondTitle.value);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void Update_ExistingPostWithoutCache_ModifiesFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var entryId = Guid.NewGuid().ToString();
				SaveEntryDirect(Path.Combine(sandbox.TestEnvironmentPath, Constants.ContentDirectory)
				  ,entryId);
				// create the file and seed with an uncached post
				var blogManager = platform.CreateBlogManager(sandbox);
				var entry = MakeMiniimalEntry();
				entry.Title = "updated";
				entry.EntryId = entryId;
				blogManager.UpdateEntry(entry);
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				var savedTitle = testDataProcessor.GetBlogPostValue(DateTime.Today, entry.EntryId, "Title");
				Assert.Equal("updated", savedTitle.value);
			}
		}

		/// <summary>
		/// writes day entry file into the content directory 
		/// </summary>
		/// <param name="directory">e.g. c:/projects/dasblog-core.../Environments/Vanilla/content</param>
		private void SaveEntryDirect(string directory, string entryId = null)
		{
			var fileName = TestDataProcesor.GetBlogEntryFileName(DateTime.Today);
			var path = Path.Combine(directory, fileName);
			var str = string.Format(minimalBlogPostXml
			  , DateTime.Today.ToString("s", CultureInfo.InvariantCulture)
			  , DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
			  , DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
			  , entryId ?? Guid.NewGuid().ToString());
			File.WriteAllText(path, str);
		}

		private static Entry MakeMiniimalEntry()
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