using DasBlog.Core.Common;
using DasBlog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Security.Policy;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Layout
{
	public class SiteSearchBoxTagHelper : TagHelper
	{
		public string Id { get; set; } = "searchText";
		public string Class { get; set; } = "col-md-3";
		public string InnerClass { get; set; } = "pull-right affix";
		public string ActionRoute { get; set; }
		public string Action { get; set; }
		public string Heading { get; set; } = "Search";
		public string ButtonName { get; set; } = "Go";
		public string ButtonClass { get; set; } = "btn";

		private readonly IUrlHelper urlHelper;
		private readonly IDasBlogSettings dasBlogSettings;

		public SiteSearchBoxTagHelper(IDasBlogSettings dasBlogSettings, IHttpContextAccessor accessor)
		{
			this.dasBlogSettings = dasBlogSettings;
			urlHelper = accessor.HttpContext?.Items[typeof(IUrlHelper)] as IUrlHelper;
			if (urlHelper == null)
			{
				throw new Exception("No UrlHelper found in http context");
			}
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			string actionUrl;
			if (ActionRoute != null)
			{
				actionUrl = urlHelper.RouteUrl(ActionRoute);
			}
			else if (Action != null)
			{
				actionUrl = Action;
			}
			else
			{
				var baseUri = new Uri(dasBlogSettings.SiteConfiguration.Root);
				var myUri = new Uri(baseUri, urlHelper.RouteUrl(Constants.SearcherRouteName));

				actionUrl = myUri.AbsoluteUri;
			}

			output.TagName = string.Empty;
			output.Content.Clear();
			output.Content.AppendHtml($@"
				<div class='{Class}'>
					<div class='{InnerClass}'>
						<div class='card'>
							<div class='card-header'>{Heading}</div>
								<div class='card-body'>
									<form method='post' action='{actionUrl}'>
										<input type='text' name='searchText' id='{Id}'/>
										<input class='{ButtonClass}' type='submit' value='{ButtonName}'/>
									</form>
								</div>
							</div>
						</div>
					</div>
				</div>
										");
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
