using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers.Post;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	[Obsolete]
	public class TitleLinkTagHelper : PostTitleLinkTagHelper
	{
		public TitleLinkTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
		}
	}
}
