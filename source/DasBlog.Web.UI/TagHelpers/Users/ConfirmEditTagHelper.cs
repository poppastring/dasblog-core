using System.Security.Principal;
using System.Threading.Tasks;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Users
{
	/// <summary>
	/// tried very hard to get this to deriive from InputTagHelper.
	/// Just delegating process without implementing the logic for this class
	/// an exception was thrown.  After doing CopyHtmlAttributes for the Value
	/// attribute it had a fit and tried to acccess some null SynchronisationContext
	/// thingie.  async/await may have been at the back of it but I suppose
	/// I shouldn't go around deriving from concrete classes - all very character building.
	/// </summary>
	public class ConfirmEditTagHelper : TagHelper
	{
		[HtmlAttributeName("type")]
		public string InputTypeName { get; set; }
		public string Value { get; set; }
		public string Name { get; set; }
		public string Id { get; set; }
		public string Class { get; set; }
		public string OnClick { get; set; }
		private readonly IHttpContextAccessor _httpContextAccessor;
		
		public ConfirmEditTagHelper(IHtmlGenerator generator, IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = "input";
			output.TagMode = TagMode.SelfClosing;
			output.Attributes.Add("type", InputTypeName);
			output.Attributes.Add("value", Value);
			output.Attributes.Add("name", Name);
			output.Attributes.Add("id", Id);
			output.Attributes.Add("class", Class);
			output.Attributes.Add("onclick", OnClick);
		}

	}
}
