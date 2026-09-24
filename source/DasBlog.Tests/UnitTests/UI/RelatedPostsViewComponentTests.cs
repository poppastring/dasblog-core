using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Moq;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class RelatedPostsViewComponentTests
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void Invoke_RanksBySharedCategoriesThenRecency()
		{
			var now = DateTime.UtcNow;
			var entries = new EntryCollection
			{
				CreateEntry("one-match-newer", "One match newer", "dotnet", now.AddDays(-1)),
				CreateEntry("two-matches", "Two matches", "dotnet;testing", now.AddDays(-10)),
				CreateEntry("one-match-older", "One match older", "testing", now.AddDays(-2)),
				CreateEntry("no-match", "No match", "design", now.AddHours(-1))
			};
			var component = CreateComponent(entries);
			var post = CreateCurrentPost("current", "dotnet", "testing");

			var result = Assert.IsType<ViewViewComponentResult>(component.Invoke(post));
			var model = Assert.IsType<RelatedPostsViewModel>(result.ViewData.Model);

			Assert.Equal(
				new[] { "two-matches", "one-match-newer", "one-match-older", "no-match" },
				model.Posts.Select(relatedPost => relatedPost.Url));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Invoke_ExcludesCurrentPrivateAndFuturePosts()
		{
			var now = DateTime.UtcNow;
			var current = CreateEntry("current", "Current", "dotnet", now.AddDays(-2));
			var privatePost = CreateEntry("private", "Private", "dotnet", now.AddDays(-3), false);
			var futurePost = CreateEntry("future", "Future", "dotnet", now.AddDays(1));
			var publicPost = CreateEntry("public", "Public", "dotnet", now.AddDays(-1));
			var component = CreateComponent(new EntryCollection { current, privatePost, futurePost, publicPost });

			var result = Assert.IsType<ViewViewComponentResult>(component.Invoke(CreateCurrentPost("current", "dotnet")));
			var model = Assert.IsType<RelatedPostsViewModel>(result.ViewData.Model);

			var relatedPost = Assert.Single(model.Posts);
			Assert.Equal("public", relatedPost.Url);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Invoke_UsesEntryIdToOrderEqualScoresAndDatesDeterministically()
		{
			var createdUtc = DateTime.UtcNow.AddDays(-1);
			var entries = new EntryCollection
			{
				CreateEntry("c", "Same title", "dotnet", createdUtc),
				CreateEntry("a", "Same title", "dotnet", createdUtc),
				CreateEntry("b", "Same title", "dotnet", createdUtc)
			};
			var component = CreateComponent(entries);

			var result = Assert.IsType<ViewViewComponentResult>(component.Invoke(CreateCurrentPost("current", "dotnet")));
			var model = Assert.IsType<RelatedPostsViewModel>(result.ViewData.Model);

			Assert.Equal(new[] { "a", "b", "c" }, model.Posts.Select(relatedPost => relatedPost.Url));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Invoke_AppliesResultLimit()
		{
			var now = DateTime.UtcNow;
			var entries = new EntryCollection
			{
				CreateEntry("first", "First", "dotnet", now.AddDays(-1)),
				CreateEntry("second", "Second", "dotnet", now.AddDays(-2)),
				CreateEntry("third", "Third", "dotnet", now.AddDays(-3))
			};
			var component = CreateComponent(entries);

			var result = Assert.IsType<ViewViewComponentResult>(component.Invoke(CreateCurrentPost("current", "dotnet"), 2));
			var model = Assert.IsType<RelatedPostsViewModel>(result.ViewData.Model);

			Assert.Equal(new[] { "first", "second" }, model.Posts.Select(relatedPost => relatedPost.Url));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Invoke_ReturnsEmptyContentWhenNoRelatedPostsExist()
		{
			var component = CreateComponent(new EntryCollection
			{
				CreateEntry("current", "Current", "dotnet", DateTime.UtcNow.AddDays(-1))
			});

			var result = Assert.IsType<ContentViewComponentResult>(component.Invoke(CreateCurrentPost("current", "dotnet")));

			Assert.Equal(string.Empty, result.Content);
		}

		private static RelatedPostsViewComponent CreateComponent(EntryCollection entries)
		{
			var blogManager = new Mock<IBlogManager>();
			blogManager.Setup(manager => manager.GetAllEntries()).Returns(entries);

			var urlResolver = new Mock<IUrlResolver>();
			urlResolver.Setup(resolver => resolver.GeneratePostUrl(It.IsAny<Entry>()))
				.Returns((Entry entry) => entry.EntryId);
			urlResolver.Setup(resolver => resolver.RelativeToRoot(It.IsAny<string>()))
				.Returns((string url) => url);

			return new RelatedPostsViewComponent(blogManager.Object, urlResolver.Object);
		}

		private static Entry CreateEntry(string entryId, string title, string categories, DateTime createdUtc, bool isPublic = true)
		{
			return new Entry
			{
				EntryId = entryId,
				Title = title,
				Categories = categories,
				CreatedUtc = createdUtc,
				IsPublic = isPublic
			};
		}

		private static PostViewModel CreateCurrentPost(string entryId, params string[] categories)
		{
			return new PostViewModel
			{
				EntryId = entryId,
				Categories = categories
					.Select(category => new CategoryViewModel { Category = category })
					.ToList()
			};
		}
	}
}
