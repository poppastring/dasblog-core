using DasBlog.Web.TagHelpers.Comments;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class ApproveCommentTagHelper : CommentApprovalLinkTagHelper
	{
		public ApproveCommentTagHelper(IUrlHelperFactory urlHelperFactory) : base(urlHelperFactory)
		{
		}
	}
}
