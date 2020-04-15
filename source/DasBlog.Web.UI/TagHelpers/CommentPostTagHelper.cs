using DasBlog.Services;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class CommentPostTagHelper : PostCommentLinkTagHelper
	{
		public CommentPostTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
		}
	}
}
