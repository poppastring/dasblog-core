using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class CommentPostTagHelper : PostCommentLinkTagHelper
	{
		public CommentPostTagHelper(ISiteConfig siteConfig, IUrlResolver urlResolver) : base(siteConfig, urlResolver)
		{
		}
	}
}
