using System;
using DasBlog.Tests.Support.Interfaces;
using Constants = DasBlog.Tests.Support.Common.Constants;
using Xunit;
using Xunit.Abstractions;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class BlogManagerTests : IClassFixture<ComponentTestPlatform>, IDisposable
	{

		private ComponentTestPlatform platform;
		private IDasBlogSandbox dasBlogSandbox;
		public BlogManagerTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)
		{
			componentTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = componentTestPlatform;
			dasBlogSandbox = platform.CreateSandbox(Constants.VanillaEnvironment);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingBlog_WithUnmatchableData_ReturnsNull()
		{
			var sandbox = dasBlogSandbox;
			var blogManager = platform.CreateBlogManager(sandbox);
			EntryCollection entries = blogManager.SearchEntries("this cannot be found", null);
			Assert.Empty(entries);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingBlog_WithMatchableData_ReturnsOneRecord()
		{
			var blogManager = platform.CreateBlogManager(dasBlogSandbox);
			EntryCollection entries = blogManager.SearchEntries("put some text here", null);
			Assert.Single(entries);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void LoadingBlogPost_ForValidEntry_ReturnsEntry()
		{
			const string entryId = "5125c596-d6d5-46fe-9f9b-c13f851d8b0d";
			const string title = "created with unique on";
			var blogManager = platform.CreateBlogManager(dasBlogSandbox);
			Entry entry = blogManager.GetEntryForEdit(entryId);
			Assert.NotNull(entry);
			Assert.Equal(entryId, entry.EntryId);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void LoadingBlogPost_ForValidEntryOnDate_ReturnsEntry()
		{
			const string entryId = "5125c596-d6d5-46fe-9f9b-c13f851d8b0d";
			const string compressedTitle = "createdwithuniqueon";
			DateTime dt = new DateTime(2018,8,3);
			var blogManager = platform.CreateBlogManager(dasBlogSandbox);
			Entry entry = blogManager.GetBlogPost(compressedTitle, dt);
			Assert.NotNull(entry);
			Assert.Equal(entryId, entry.EntryId);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void LoadingBlogPost_ForValidEntryNoDate_ReturnsEntry()
		{
			const string entryId = "5125c596-d6d5-46fe-9f9b-c13f851d8b0d";
			const string compressedTitle = "createdwithuniqueon";
			var blogManager = platform.CreateBlogManager(dasBlogSandbox);
			Entry entry = blogManager.GetBlogPost(compressedTitle, null);
			Assert.NotNull(entry);
			Assert.Equal(entryId, entry.EntryId);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void FrontPage_ForFewerThanMaxEntries_ReturnsAllEntries()
		{
			var blogManager = platform.CreateBlogManager(dasBlogSandbox);
			EntryCollection entries = blogManager.GetFrontPagePosts(null);
			Assert.Equal(2, entries.Count);
		}
		public void Dispose()
		{
			dasBlogSandbox?.Terminate();
		}
	}
}
