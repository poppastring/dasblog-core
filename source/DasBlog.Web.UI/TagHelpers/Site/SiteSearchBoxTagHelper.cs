using DasBlog.Services;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.TagHelpers.Layout
{
	public class SiteSearchBoxTagHelper : SearchBoxTagHelper
	{
		public SiteSearchBoxTagHelper(IHttpContextAccessor accessor) : base(accessor)
		{

		}
	}
}
