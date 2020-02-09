using DasBlog.Services;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostCommentLinkTagHelper : CommentPostTagHelper
	{
		public PostCommentLinkTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{

		}
	}
}
