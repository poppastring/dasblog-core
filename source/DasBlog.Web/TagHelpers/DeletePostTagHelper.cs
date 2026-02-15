using DasBlog.Services;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class DeletePostTagHelper : PostDeleteLinkTagHelper
	{
		public DeletePostTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
		}
	}
}
