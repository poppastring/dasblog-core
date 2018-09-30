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
		private ITestOutputHelper testOutputHelper;
		private IDasBlogSandbox dasBlogSandbox;
		public BlogManagerTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)
		{
			componentTestPlatform.CompleteSetup(testOutputHelper);
			this.platform = componentTestPlatform;
			this.testOutputHelper = testOutputHelper;
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
			IDasBlogSandbox sandbox = dasBlogSandbox;
			sandbox = platform.CreateSandbox(Constants.VanillaEnvironment);
			sandbox.Init();
			var blogManager = platform.CreateBlogManager(sandbox);
			EntryCollection entries = blogManager.SearchEntries("put some text here", null);
			Assert.Single(entries);
		}


		public void Dispose()
		{
			dasBlogSandbox?.Terminate();
		}
	}
}
