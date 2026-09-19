using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class PostToTwitterTagHelperTests
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ")]
		[Trait("Category", "UnitTest")]
		public async Task ProcessAsync_BlankTwitterSite_OmitsTwitterReference(string twitterSite)
		{
			var output = await ProcessPostToTwitterTagHelper(twitterSite);
			var href = output.Attributes["href"].Value.ToString();

			Assert.Equal("a", output.TagName);
			Assert.DoesNotContain("via=", href);
			Assert.Contains("text=Post%20title", href);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task ProcessAsync_ConfiguredTwitterSite_UsesConfiguredHandle()
		{
			var output = await ProcessPostToTwitterTagHelper("@configured");
			var href = output.Attributes["href"].Value.ToString();

			Assert.Contains("via=configured", href);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task ProcessAsync_RenderedHrefEncodesQuerySeparatorsOnce()
		{
			var output = await ProcessPostToTwitterTagHelper("@configured");
			using var writer = new StringWriter();

			output.WriteTo(writer, HtmlEncoder.Default);
			var html = writer.ToString();

			Assert.Contains("&amp;text=Post%20title", html);
			Assert.Contains("&amp;via=configured", html);
			Assert.DoesNotContain("&amp;amp;", html);
		}

		private static async Task<TagHelperOutput> ProcessPostToTwitterTagHelper(string twitterSite)
		{
			var urlResolver = new Mock<IUrlResolver>();
			urlResolver.Setup(r => r.RelativeToRoot("post-link")).Returns("https://example.com/post-link");

			var settings = new Mock<IDasBlogSettings>();
			settings.SetupGet(s => s.MetaTags).Returns(new MetaTags { TwitterSite = twitterSite });

			var sut = new PostToTwitterTagHelper(urlResolver.Object, settings.Object)
			{
				Post = new PostViewModel
				{
					Author = "Post Author",
					PermaLink = "post-link",
					Title = "Post title",
					Categories = new List<CategoryViewModel>()
				}
			};

			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("post-to-twitter", new TagHelperAttributeList(), (_, _) =>
				Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

			await sut.ProcessAsync(context, output);

			return output;
		}
	}
}
