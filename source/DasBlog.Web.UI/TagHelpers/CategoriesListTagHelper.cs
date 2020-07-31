using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers.Post;
using System;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class CategoriesListTagHelper : PostCategoriesListTagHelper
	{
		public CategoriesListTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
		}
	}
}
