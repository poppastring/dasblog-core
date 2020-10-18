using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class PostToFacebookTagHelperTest
	{
		/// <summary>
		/// All test methods should follow this naming pattern
		/// </summary>
		[Fact]
		public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		{
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task PostToFacebookTagHelper_GeneratedTagHelper_BlogPostRendersAsAnchorTag()
		{
			var postLink = "some-great-post";
			var dasBlogSettings = new DasBlogSettingTest();

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
	}
}

