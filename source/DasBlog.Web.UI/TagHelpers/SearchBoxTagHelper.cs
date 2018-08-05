using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using DasBlog.Core.Common;

namespace DasBlog.Web.TagHelpers
{
	/// <summary>
	/// examples:
	/// &lt;search-box/&gt;
	/// &lt;search-box id='mySearchText'/&gt;
	/// &lt;search-box class='col-md6'/&gt;
	///
	/// By default the search box poss a "searchText" to its action which
	/// is /BlobPost/Search by default
	/// 	BlogPostController.Search(strng SearachText) ...
	/// You should override:
	/// 	Id - make sure the search box is unique
	/// You can override
	/// 	class - to position the seach component
	/// 	inner-class - to right justify the component
	/// 	action - if you need to temporarily push to another controller
	/// 	actionRoute - if you need to temporarlly push to another controller/action
	/// 	  takes precedence of action if both are specified
	/// 	heading - if "Search" isn't good enough for you
	/// 	button-name - if "Go" isn't good enough for you
	/// You can't override (yes, you actually have to change the code)
	/// 	inner panel structure
	/// 	the name "searchText"
	/// 	method
	/// 	classes of text box or button
	/// </summary>
	public class SearchBoxTagHelper : TagHelper
	{
		public string Id { get; set; } = "searchText";
		public string Class { get; set; } = "col-md-3";
		public string InnerClass { get; set; } = "sidebar-nav-fixed pull-right affix";
		public string ActionRoute { get; set; }
		public string Action { get; set; }
		public string Heading { get; set; } = "Search";
		public string ButtonName { get; set; } = "Go";

		private IUrlHelper urlHelper;

		public SearchBoxTagHelper(IHttpContextAccessor accessor)
		{
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
				actionUrl = urlHelper.RouteUrl(Constants.SearcherRouteName);
			}
			output.TagName = string.Empty;
			output.Content.Clear();
			output.Content.AppendHtml($@"
				<div class='{Class}'>
					<div class='{InnerClass}'>
						<div class='panel panel-default'>
							<div class='panel-heading'>{Heading}</div>
								<div class='panel-body'>
									<form method='post' action='{actionUrl}'>
										<input type='text' name='searchText' id='{Id}'/>
										<input class='btn' type='submit' value='{ButtonName}'/>
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
