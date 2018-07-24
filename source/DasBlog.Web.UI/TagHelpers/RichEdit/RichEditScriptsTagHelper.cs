using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.RichEdit
{
	public class RichEditScriptsTagHelper : TagHelper
	{
		public string ControlId { get; set; }

		private readonly IRichEditBuilder builder;

		public RichEditScriptsTagHelper(IRichEditBuilder builder)
		{
			this.builder = builder;
		}
		
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			builder.ProcessScripts(this, context, output);
		}
		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
