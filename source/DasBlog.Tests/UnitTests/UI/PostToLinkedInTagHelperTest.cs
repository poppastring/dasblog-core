using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI;

public class PostToLinkedInTagHelperTest
{
	[Fact]
	[Trait("Category", "UnitTest")]
	public async Task PostToLinkedInTagHelper_GeneratedTagHelper_BlogPostRendersAsAnchorTag()
	{
		var postLink = "what-is-lorem-ipsum";
		var dasBlogSettings = new DasBlogSettingTest();

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
}
