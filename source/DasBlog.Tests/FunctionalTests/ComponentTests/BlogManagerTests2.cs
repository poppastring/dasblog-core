using System;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using newtelligence.DasBlog.Runtime;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class BlogManagerTests2 : IClassFixture<ComponentTestPlatform>, IDisposable
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

		public void Dispose()
		{
		}
		
	}
}
