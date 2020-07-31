using System;
using DasBlog.Web.TagHelpers.Layout;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class SearchBoxTagHelper : SiteSearchBoxTagHelper
	{
		public SearchBoxTagHelper(IHttpContextAccessor accessor) : base(accessor)
		{
		}
	}
}
