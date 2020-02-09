using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostCategoriesListTagHelper : CategoriesListTagHelper
	{
		public PostCategoriesListTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
			
		}
	}
}
