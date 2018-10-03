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
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public partial class BlogManagerTests2 : IClassFixture<ComponentTestPlatform>, IDisposable
	{

		private ComponentTestPlatform platform;
		public BlogManagerTests2(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)
		{
			componentTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = componentTestPlatform;
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void FrontPage_WithEnglishLang_IgnoresFrenchEntry()
		{
			using (var sandbox = platform.CreateSandbox(Constants.LanguageEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				EntryCollection entries = blogManager.GetFrontPagePosts("en-US");
				Assert.Empty(entries);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void FrontPage_WithFrenchLang_ReturnsFrenchEntry()
		{
			using (var sandbox = platform.CreateSandbox(Constants.LanguageEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				EntryCollection entries = blogManager.GetFrontPagePosts("fr-FR");
				Assert.Single(entries);
			}
		}
		[Fact(Skip="true")]
		[Trait(Constants.DescriptionTraitType, "returns entry irrespective of date")]
		[Trait(Constants.FailureTraitTraitType, Constants.ApiFailureTraitValue)]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void GetBlogPost_ForValidEntryOnWrongDate_ReturnsNull()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				const string compressedTitle = "createdwithuniqueon";
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				testDataProcessor.SetSiteConfigValue("EnableTitlePermaLinkUnique", "true");
				DateTime dt = new DateTime(2018,8,5);
				var blogManager = platform.CreateBlogManager(sandbox);
				Entry entry = blogManager.GetBlogPost(compressedTitle, dt);
				Assert.Null(entry);
			}
		}
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

		/// <summary>
		/// writes day entry file into the content directory 
		/// </summary>
		/// <param name="directory">e.g. c:/projects/dasblog-core.../Environments/Vanilla/content</param>
		private void SaveEntryDirect(string directory)
		{
			var fileName = TestDataProcesor.GetBlogEntryFileName(DateTime.Today);
			var path = Path.Combine(directory, fileName);
			var str = string.Format(minimalBlogPostXml
			  , DateTime.Today.ToString("s", CultureInfo.InvariantCulture)
			  , DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
			  , DateTime.Now.ToString("s", CultureInfo.InvariantCulture)
			  , Guid.NewGuid().ToString());
			File.WriteAllText(path, str);
		}

		public void Dispose()
		{
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
