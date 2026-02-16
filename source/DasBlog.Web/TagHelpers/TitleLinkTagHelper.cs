using DasBlog.Services;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class TitleLinkTagHelper : PostTitleLinkTagHelper
	{
		public TitleLinkTagHelper(IUrlResolver urlResolver) : base(urlResolver)
		{
		}
	}
}
