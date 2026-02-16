using System;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.TagHelpers.Layout;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class SearchBoxTagHelper : SiteSearchBoxTagHelper
	{
		public SearchBoxTagHelper(ISiteConfig siteConfig, IHttpContextAccessor accessor) : base(siteConfig, accessor)
		{
		}
	}
}
