using System;
using DasBlog.Tests.Support.Interfaces;
using Constants = DasBlog.Tests.Support.Common.Constants;
using Xunit;
using Xunit.Abstractions;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public partial class BlogManagerTests : IClassFixture<ComponentTestPlatform>, IDisposable
	{

		private ComponentTestPlatform platform;
		public BlogManagerTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)
		{
			componentTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = componentTestPlatform;
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingBlog_WithUnmatchableData_ReturnsNull()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				EntryCollection entries = blogManager.SearchEntries("this cannot be found", null);
				Assert.Empty(entries);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingBlog_WithMatchableData_ReturnsOneRecord()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				EntryCollection entries = blogManager.SearchEntries("put some text here", null);
				Assert.Single(entries);
			}
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void LoadingBlogPost_ForValidEntry_ReturnsEntry()
		{
			const string entryId = "5125c596-d6d5-46fe-9f9b-c13f851d8b0d";
			const string title = "created with unique on";
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				Entry entry = blogManager.GetEntryForEdit(entryId);
				Assert.NotNull(entry);
				Assert.Equal(entryId, entry.EntryId);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void LoadingBlogPost_ForValidEntryOnDate_ReturnsEntry()
		{
			const string entryId = "5125c596-d6d5-46fe-9f9b-c13f851d8b0d";
			const string compressedTitle = "createdwithuniqueon";
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				DateTime dt = new DateTime(2018, 8, 3);
				var blogManager = platform.CreateBlogManager(sandbox);
				Entry entry = blogManager.GetBlogPost(compressedTitle, dt);
				Assert.NotNull(entry);
				Assert.Equal(entryId, entry.EntryId);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void LoadingBlogPost_ForValidEntryNoDate_ReturnsEntry()
		{
			const string entryId = "5125c596-d6d5-46fe-9f9b-c13f851d8b0d";
			const string compressedTitle = "createdwithuniqueon";
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				Entry entry = blogManager.GetBlogPost(compressedTitle, null);
				Assert.NotNull(entry);
				Assert.Equal(entryId, entry.EntryId);
			}
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void FrontPage_ForFewerThanMaxEntries_ReturnsAllEntries()
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				EntryCollection entries = blogManager.GetFrontPagePosts(null);
				Assert.Equal(2, entries.Count);
			}
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
		public void Dispose()
		{
		}
	}
}
