using System;
using System.IO;
using System.Threading;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Common;
using Xunit;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.ComponentTests
{
	public partial class BlogManagerTests
	{
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void GetCategories_ForMultipleDays_ReturnsAllCategories()
		{
			var str = Thread.GetDomain().FriendlyName;
			using (var sandbox = platform.CreateSandbox(Constants.CategoriesEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				CategoryCacheEntryCollection categories = blogManager.GetCategories();
				Assert.Equal(2, categories.Count);
			}
		}
		[Fact(Skip="")]
		// when this runs before GetCategories_ForMultipleDays_ReturnsAllCategories
		// which it does due to the runner's sort order then the other test fails
		// because of caching issues
		[Trait(Constants.FailureTraitType, Constants.FailsInSuiteTraitValue)]
		[Trait(Constants.DescriptionTraitType, "throws exception when run as part of suite")]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void GetCategories_ForEmptyContentDiretory_ReturnsNothing()
		{
			using (var sandbox = platform.CreateSandbox(Constants.EmptyContentEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				CategoryCacheEntryCollection categories = blogManager.GetCategories();
				Assert.Empty(categories);
			}
		}
	}
}
