using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DasBlog.Web.UI.Helpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using Xunit;

namespace DasBlog.Tests.UI
{
	public class TagHelper_Test
	{
		/// <summary>
		/// All test methods should follow this naming pattern
		/// </summary>
		[Fact]
		public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		{

		}

		[Fact]
		public void EditPost_Html_RendersCorrectly()
		{
			string blogPost = "theBlogPost";
			var context = new TagHelperContext(new TagHelperAttributeList { { "Blog", blogPost } }, new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput("editpost", new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
			{
				var tagHelperContent = new DefaultTagHelperContent();
				tagHelperContent.SetContent(string.Empty);
				return Task.FromResult<TagHelperContent>(tagHelperContent);
			});


			var helper = new EditPostTagHelper { Blog = blogPost };
			helper.Process(context, output);

			Assert.Same("a", output.TagName);
			Assert.Same("href", output.Attributes[0].Name);
			Assert.Contains(blogPost, output.Attributes[0].Value.ToString());
		}
	}
}

