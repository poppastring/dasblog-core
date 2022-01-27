using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers.Comments;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI;

public class CommentContentTagHelperTest
{

	[Fact]
	[Trait("Category", "UnitTest")]
	public void CommentContentTagHelper_WhenCommentIsNull_ShouldNotThrow()
	{
		var dasBlogSettings = new DasBlogSettingTest();

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
		var dasBlogSettings = new DasBlogSettingTest();

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
}
