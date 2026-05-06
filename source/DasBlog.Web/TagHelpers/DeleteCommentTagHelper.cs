using DasBlog.Web.TagHelpers.Comments;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class DeleteCommentTagHelper : CommentDeleteLinkTagHelper
	{
		public DeleteCommentTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory)
		{
		}
	}
}
