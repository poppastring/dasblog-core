using System;
using System.IO;
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
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void AddComment_ForFirstComent_CreatesDayFeedbackFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				testDataProcessor.SetSiteConfigValue("DaysCommentsAllowed", "9999");
				var blogManager = platform.CreateBlogManager(sandbox);
				var comment = MakeMiniimalComment("entry-id-2018-02-25-0001");
				blogManager.AddComment("entry-id-2018-02-25-0001", comment);
				Assert.True(DayFeedbackFileExists(Path.Combine(sandbox.TestEnvironmentPath, Constants.ContentDirectory)
				  ,new DateTime(2018, 2, 25) ));
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void AddComment_ForFurtherComent_ModifiesDayFeedbackFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				testDataProcessor.SetSiteConfigValue("DaysCommentsAllowed", "9999");
				var blogManager = platform.CreateBlogManager(sandbox);
				var comment = MakeMiniimalComment("b705c37b-b47f-4e8d-8f8b-091efc4cb684");
				comment.EntryId = "comment-id-2018-02-24-0001";
				blogManager.AddComment("b705c37b-b47f-4e8d-8f8b-091efc4cb684", comment);
				Assert.True(DayFeedbackFileExists(Path.Combine(sandbox.TestEnvironmentPath, Constants.ContentDirectory)
				  ,new DateTime(2018, 2, 24) ));
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void DeleteComment_ForNewlyAddedComent_RemovesCommentFromFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				testDataProcessor.SetSiteConfigValue("DaysCommentsAllowed", "9999");
				var blogManager = platform.CreateBlogManager(sandbox);
				var comment = MakeMiniimalComment("entry-id-2018-02-25-0001");
				comment.EntryId = "comment-id-2018-02-25-0001";
				blogManager.AddComment("entry-id-2018-02-25-0001", comment);
				var savedCommentId = testDataProcessor.GetDayExtraValue(new DateTime(2018, 2, 25), "comment-id-2018-02-25-0001", "EntryId");
				Assert.Equal("comment-id-2018-02-25-0001", savedCommentId.value);
				blogManager.DeleteComment("entry-id-2018-02-25-0001", "comment-id-2018-02-25-0001");
				savedCommentId = testDataProcessor.GetDayExtraValue(new DateTime(2018, 2, 25), "EntryId", "comment-id-2018-02-25-0001");
				Assert.False(savedCommentId.success);
			}
		}

		private static Comment MakeMiniimalComment(string entryId)
		{
			Comment comment = new Comment();
			comment.Initialize();
			comment.Author = "Steinbeck";
			comment.Content = "minimal";
			comment.TargetTitle = string.Empty;
			comment.TargetEntryId = entryId;
			comment.EntryId = "comment-id-2018-02-25-0001";
			return comment;
		}

	}
}
