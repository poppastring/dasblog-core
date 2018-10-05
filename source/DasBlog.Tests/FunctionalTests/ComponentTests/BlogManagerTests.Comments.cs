using System;
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
		public void SearchingComments_WithUnmatchableData_ReturnsEmptyCollection()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				CommentCollection comments = blogManager.GetComments("this cannot be found", true);
				Assert.Empty(comments);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingPublicComments_InPrivateData_ReturnsPublicOnly()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				CommentCollection comments = blogManager.GetComments("b705c37b-b47f-4e8d-8f8b-091efc4cb684", false);
				Assert.Single(comments);
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void SearchingComments_ForExistingData_ReturnsComment()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var blogManager = platform.CreateBlogManager(sandbox);
				CommentCollection comments = blogManager.GetComments("b705c37b-b47f-4e8d-8f8b-091efc4cb684", true);
				Assert.Equal(2, comments.Count);
			}
		}

		
	}
}
