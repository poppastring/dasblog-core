using DasBlog.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement(Attributes = "blogfeature")]
	public class BlogFeatureTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public BlogFeatureTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBlogFeatures)
			{
				output.SuppressOutput();
				return;
			}

			output.Attributes.RemoveAll("blogfeature");
		}
	}
}
