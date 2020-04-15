using DasBlog.Services;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class EditPostTagHelper : PostEditLinkTagHelper
	{
		public EditPostTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{

		}
	}
}
