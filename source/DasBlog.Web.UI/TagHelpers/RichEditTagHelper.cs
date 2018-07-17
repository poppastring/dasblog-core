using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
	public class RichEditTagHelper : TagHelper
	{
		public string Id { get; set; }
		public string Name { get; set; }

		private readonly IRichEditBuilder builder;

		public RichEditTagHelper(IRichEditBuilder builder)
		{
			this.builder = builder;
		}
		
		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			builder.ProcessControl(this, context, output);
		}
		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
