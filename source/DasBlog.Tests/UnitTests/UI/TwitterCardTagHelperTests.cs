using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.TagHelpers.Site;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class TwitterCardTagHelperTests
	{
		private static string ProcessTwitterCardTagHelper(MetaTags metaTags, Action<ViewContext> configureViewContext = null)
		{
			var settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.SetupGet(s => s.MetaTags).Returns(metaTags);

			var viewContext = new ViewContext();
			configureViewContext?.Invoke(viewContext);

			var sut = new TwitterCardTagHelper(settingsMock.Object)
			{
				ViewContext = viewContext
			};

			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("twitter-card", new TagHelperAttributeList(), (_, _) =>
				Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

			sut.Process(context, output);
			return output.Content.GetContent();
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Process_BlankValues_EmitsOnlyFallbackTwitterCard()
		{
			var html = ProcessTwitterCardTagHelper(new MetaTags());

			Assert.Contains("<meta name=\"twitter:card\" content=\"summary\" />", html);
			Assert.DoesNotContain("twitter:site", html);
			Assert.DoesNotContain("twitter:creator", html);
			Assert.DoesNotContain("twitter:title", html);
			Assert.DoesNotContain("twitter:description", html);
			Assert.DoesNotContain("twitter:image", html);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Process_InvalidTwitterCardValue_UsesSummaryFallback()
		{
			var html = ProcessTwitterCardTagHelper(new MetaTags { TwitterCard = "invalid" });

			Assert.Contains("<meta name=\"twitter:card\" content=\"summary\" />", html);
			Assert.DoesNotContain("summary_large_image", html);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Process_PageImageMissing_UsesConfiguredTwitterImage()
		{
			var html = ProcessTwitterCardTagHelper(
				new MetaTags
				{
					TwitterCard = "summary_large_image",
					TwitterImage = "https://example.com/default-image.png"
				},
				vc => vc.ViewData["PageImageUrl"] = string.Empty);

			Assert.Contains("<meta name=\"twitter:card\" content=\"summary_large_image\" />", html);
			Assert.Contains("<meta name=\"twitter:image\" content=\"https://example.com/default-image.png\" />", html);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Process_HandleWithoutAtPrefix_NormalizesWithAtPrefix()
		{
			var html = ProcessTwitterCardTagHelper(
				new MetaTags
				{
					TwitterCard = "summary",
					TwitterSite = "poppastring",
					TwitterCreator = "@mark"
				},
				vc =>
				{
					vc.ViewData["PageTitle"] = "Post title";
					vc.ViewData["Description"] = "Post description";
				});

			Assert.Contains("<meta name=\"twitter:site\" content=\"@poppastring\" />", html);
			Assert.Contains("<meta name=\"twitter:creator\" content=\"@mark\" />", html);
			Assert.Contains("<meta name=\"twitter:title\" content=\"Post title\" />", html);
			Assert.Contains("<meta name=\"twitter:description\" content=\"Post description\" />", html);
		}
	}
}
