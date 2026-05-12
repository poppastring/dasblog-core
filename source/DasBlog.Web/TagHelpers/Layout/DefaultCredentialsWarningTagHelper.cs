using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using DasBlog.Web.Services;

namespace DasBlog.Web.TagHelpers.Layout
{
	[HtmlTargetElement("default-credentials-warning", TagStructure = TagStructure.WithoutEndTag)]
	public class DefaultCredentialsWarningTagHelper : TagHelper
	{
		private readonly IDefaultCredentialsCheck _defaultCredentialsCheck;
		private readonly IUrlHelperFactory _urlHelperFactory;

		public DefaultCredentialsWarningTagHelper(
			IDefaultCredentialsCheck defaultCredentialsCheck,
			IUrlHelperFactory urlHelperFactory)
		{
			_defaultCredentialsCheck = defaultCredentialsCheck;
			_urlHelperFactory = urlHelperFactory;
		}

		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			output.TagMode = TagMode.StartTagAndEndTag;

			var user = ViewContext.HttpContext.User;
			if (user?.Identity?.IsAuthenticated != true || !user.IsInRole("admin"))
			{
				output.SuppressOutput();
				return;
			}

			if (!_defaultCredentialsCheck.IsUsingDefaults())
			{
				output.SuppressOutput();
				return;
			}

			var urlHelper = _urlHelperFactory.GetUrlHelper(ViewContext);
			var href = urlHelper.Action(new UrlActionContext { Action = "Index", Controller = "Author" }) ?? "#";

			var sb = new StringBuilder();
			sb.Append("<div class=\"alert alert-danger mb-0 text-center\" role=\"alert\">");
			sb.Append("<strong>&#9888; Security Warning:</strong> ");
			sb.Append("You are still using the default admin email and/or password. ");
			sb.Append("<a href=\"").Append(href).Append("\" class=\"alert-link\">Update them now</a> ");
			sb.Append("to secure your blog.");
			sb.Append("</div>");

			output.Content.SetHtmlContent(sb.ToString());
		}
	}
}
