using DasBlog.Services;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class CategoriesListTagHelper : PostCategoriesListTagHelper
	{
		public CategoriesListTagHelper(IUrlResolver urlResolver) : base(urlResolver)
		{
		}
	}
}
