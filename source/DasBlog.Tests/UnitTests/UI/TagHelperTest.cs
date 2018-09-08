using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class TagHelperTest
	{
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
		public void DasBlogTagHelper_GeneratedTagHelper_BlogPostRendersAsAnchorTag(TagHelper helper, string blogPost)
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
		}

		public static TheoryData<TagHelper, string> DasBlogPostLinkTagHelperData = new TheoryData<TagHelper, string>
		{
			{new EditPostTagHelper(new DasBlogSettingTest()) { BlogPostId = "theBlogPost"}, "theBlogPost"} ,
			{new CommentPostTagHelper(new DasBlogSettingTest()) { Post = new PostViewModel(){ PermaLink = "some-blog-post", EntryId = "0B74C9D3-4D2C-4754-B607-F3847183221C" }}, "http://www.poppastring.com/post/0B74C9D3-4D2C-4754-B607-F3847183221C/comments" }
		};
	}
}

