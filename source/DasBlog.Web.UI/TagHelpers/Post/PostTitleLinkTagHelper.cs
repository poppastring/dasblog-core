using DasBlog.Services;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostTitleLinkTagHelper : TitleLinkTagHelper
	{
		public PostTitleLinkTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{

		}
	}
}
