using DasBlog.Services;
using DasBlog.Web.TagHelpers.Comments;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
    public class ApproveCommentTagHelper : CommentApprovalLinkTagHelper
	{
		public ApproveCommentTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
		}
	}
}
