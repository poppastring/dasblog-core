using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

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
		public string Action { get; set; } = "/BlogPost/Search";
				// TODO user route analysis to prvoide action
		public string Heading { get; set; } = "Search";
		public string ButtonName { get; set; } = "Go";
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
//			output.TagMode = TagMode.SelfClosing;
			output.TagName = string.Empty;
			output.Content.Clear();
			output.Content.AppendHtml($@"
				<div class='{Class}'>
					<div class='{InnerClass}'>
						<div class='panel panel-default'>
							<div class='panel-heading'>{Heading}</div>
								<div class='panel-body'>
									<form method='post' action='{Action}'>
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
