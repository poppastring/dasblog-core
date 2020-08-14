using System;
using DasBlog.Services;
using DasBlog.Web.TagHelpers.Layout;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class SearchBoxTagHelper : SiteSearchBoxTagHelper
	{
		public SearchBoxTagHelper(IDasBlogSettings dasBlogSettings, IHttpContextAccessor accessor) : base(dasBlogSettings, accessor)
		{
		}
	}
}
