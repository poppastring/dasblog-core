using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using DasBlog.Web.TagHelpers;
using DasBlog.Web.TagHelpers.Comments;
using DasBlog.Web.TagHelpers.Post;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class TagHelperTest
	{
		public static DasBlogSettings dasBlogSettings = new DasBlogSettingsMock().CreateSettings();

		/// <summary>
		/// All test methods should follow this naming pattern
		/// </summary>
		[Fact]
		public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		{
		}

		[Theory]
		[MemberData(nameof(DasBlogPostLinkTagHelperData))]
		[Trait("Category", "UnitTest")]
		public void DasBlogTagHelper_GeneratedTagHelper_BlogPostRendersAsAnchorTag(TagHelper helper, string blogPost, string tagContent)
		{
			var context = new TagHelperContext(new TagHelperAttributeList { { "BlogPostId", blogPost } }, new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("editpost", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

			helper.Process(context, output);

			Assert.Same("a", output.TagName);
			Assert.Same("href", output.Attributes[0].Name);
			Assert.Contains(blogPost, output.Attributes[0].Value.ToString());
			Assert.DoesNotContain("id", output.Attributes.Select(a => a.Name));
			Assert.Equal(tagContent, output.Content.GetContent().Trim());
		}

		public static TheoryData<TagHelper, string, string> DasBlogPostLinkTagHelperData = new TheoryData<TagHelper, string, string>
		{
			{new PostEditLinkTagHelper(dasBlogSettings) {BlogPostId = "theBlogPost"}, "theBlogPost", "Edit this post"},
			{new PostCommentLinkTagHelper(dasBlogSettings) { Post = new PostViewModel { PermaLink = "some-blog-post", EntryId = "0B74C9D3-4D2C-4754-B607-F3847183221C" }}, "/some-blog-post/comments", "Comment on this post [0]" },
			{new PostCommentLinkTagHelper(dasBlogSettings) { Post = new PostViewModel { PermaLink = "some-blog-post", EntryId = "0B74C9D3-4D2C-4754-B607-F3847183221C" }, LinkText = "Custom text ({0})"}, "/some-blog-post/comments", "Custom text (0)" },
			{new PostCommentLinkTagHelper(dasBlogSettings) { Post = new PostViewModel { PermaLink = "some-blog-post", EntryId = "0B74C9D3-4D2C-4754-B607-F3847183221C" }, LinkText = "Link text only "}, "/some-blog-post/comments", "Link text only" }
		};

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task PostToFacebookTagHelper_GeneratedTagHelper_BlogPostRendersAsAnchorTag()
		{
			var postLink = "some-great-post";
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();

			var sut = new PostToFacebookTagHelper(dasBlogSettings) { Post = new PostViewModel { PermaLink = postLink, EntryId = "F79DCB23-1536-4496-B6F3-109F05DEEE10" } };

			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("editpost", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

			await sut.ProcessAsync(context, output);

			Assert.Same("a", output.TagName);

			var href = output.Attributes.FirstOrDefault(a => a.Name == "href")?.Value.ToString();
			Assert.NotNull(href);
			Assert.StartsWith("https://facebook.com/", href);

			var urlencodedBaseUrl = UrlEncoder.Default.Encode(dasBlogSettings.GetBaseUrl());
			Assert.Contains($"={urlencodedBaseUrl}{postLink}", href);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task PostToLinkedInTagHelper_GeneratedTagHelper_BlogPostRendersAsAnchorTag()
		{
			var postLink = "what-is-lorem-ipsum";
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();

			var sut = new PostToLinkedInTagHelper(dasBlogSettings) { Post = new PostViewModel { PermaLink = postLink, EntryId = "81c57569-3b41-46e5-a2c7-aea8681c380d" } };

			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("editpost", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

			await sut.ProcessAsync(context, output);

			Assert.Same("a", output.TagName);

			var href = output.Attributes.FirstOrDefault(a => a.Name == "href")?.Value.ToString();
			Assert.NotNull(href);
			Assert.StartsWith("https://www.linkedin.com/", href);

			var urlencodedBaseUrl = UrlEncoder.Default.Encode(dasBlogSettings.GetBaseUrl());
			Assert.Contains($"={urlencodedBaseUrl}{postLink}", href);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void CommentContentTagHelper_WhenCommentIsNull_ShouldNotThrow()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();

			var helper = new CommentContentTagHelper(dasBlogSettings);
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput(string.Empty, new TagHelperAttributeList(), (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

			helper.Process(context, output);

			Assert.Same("div", output.TagName);
			Assert.Equal("dbc-comment-content", output.Attributes[0].Value.ToString());
			Assert.Equal("", output.Content.GetContent().Trim());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void CommentContentTagHelper_WhenCommentTextIsNull_ShouldNotThrow()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings(); ;

			var helper = new CommentContentTagHelper(dasBlogSettings)
			{
				Comment = new CommentViewModel()
			};
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput(string.Empty, new TagHelperAttributeList(), (_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

			helper.Process(context, output);

			Assert.Same("div", output.TagName);
			Assert.Equal("dbc-comment-content", output.Attributes[0].Value.ToString());
			Assert.Equal("", output.Content.GetContent().Trim());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task SitePageControlTagHelper_RendersExpectedOutput()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext();
			viewContext.ViewData[DasBlog.Core.Common.Constants.PostCount] = 5;
			viewContext.ViewData[DasBlog.Core.Common.Constants.PageNumber] = 2;

			var sut = new DasBlog.Web.TagHelpers.Layout.SitePageControlTagHelper(dasBlogSettings)
			{
				ViewContext = viewContext
			};

			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("sitepagecontrol", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

			await sut.ProcessAsync(context, output);

			Assert.Equal("span", output.TagName);
			Assert.Equal("dbc-span-page-control", output.Attributes[0].Value.ToString());
			Assert.Contains("Older Posts", output.Content.GetContent());
			Assert.Contains("Newer Posts", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task PostCategoriesListTagHelper_RendersCategories()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			var categories = new List<DasBlog.Web.Models.BlogViewModels.CategoryViewModel>
			{
				new DasBlog.Web.Models.BlogViewModels.CategoryViewModel { Category = "Tech", CategoryUrl = "tech" },
				new DasBlog.Web.Models.BlogViewModels.CategoryViewModel { Category = "Life", CategoryUrl = "life" }
			};
			var post = new DasBlog.Web.Models.BlogViewModels.PostViewModel { Categories = categories };
			var sut = new DasBlog.Web.TagHelpers.Post.PostCategoriesListTagHelper(dasBlogSettings)
			{
				Post = post
			};

			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("postcategorieslist", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});

			await sut.ProcessAsync(context, output);

			Assert.Equal("span", output.TagName);
			Assert.Equal("dbc-span-category", output.Attributes[0].Value.ToString());
			Assert.Contains("Tech", output.Content.GetContent());
			Assert.Contains("Life", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void SiteDescriptionTagHelper_RendersDescription()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.Description = "Test Description";
			var sut = new DasBlog.Web.TagHelpers.SiteDescriptionTagHelper(dasBlogSettings);
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("sitedescription", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});
			sut.Process(context, output);
			Assert.Equal("Test Description", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void SiteRootTagHelper_RendersRoot()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.Root = "https://example.com/";
			var sut = new DasBlog.Web.TagHelpers.SiteRootTagHelper(dasBlogSettings);
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("siteroot", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});
			sut.Process(context, output);
			Assert.Equal("https://example.com/", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void SiteTitleTagHelper_RendersTitle()
		{
			var dasBlogSettingsMock = new DasBlogSettingsMock();
			var dasBlogSettings = dasBlogSettingsMock.CreateSettings();
			dasBlogSettings.SiteConfiguration.Title = "Test Title";
			var sut = new DasBlog.Web.TagHelpers.SiteTitleTagHelper(dasBlogSettings);
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("sitetitle", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});
			sut.Process(context, output);
			Assert.Equal("Test Title", output.Content.GetContent());
		}
	}
}

