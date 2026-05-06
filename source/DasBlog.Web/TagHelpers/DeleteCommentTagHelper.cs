using DasBlog.Services;
using DasBlog.Web.TagHelpers.Comments;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class DeleteCommentTagHelper : CommentDeleteLinkTagHelper
	{
		public DeleteCommentTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
		}
	}
}
