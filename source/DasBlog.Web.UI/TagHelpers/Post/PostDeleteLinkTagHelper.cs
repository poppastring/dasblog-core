using DasBlog.Services;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostDeleteLinkTagHelper : DeletePostTagHelper
	{
		public PostDeleteLinkTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{

		}
	}
}
