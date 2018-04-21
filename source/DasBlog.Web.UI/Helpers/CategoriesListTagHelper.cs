using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.Helpers
{
	public class CategoriesListTagHelper : TagHelper
	{
		public IList<CategoryViewModel> Categories { get; set; }

		private readonly IDasBlogSettings _dasBlogSettings;
		private const string CATEGORY_ITEM_TEMPLATE = "<li><a href='{0}'>{1}</a></li>";

		public CategoriesListTagHelper(IDasBlogSettings dasBlogSettings)
		{
			_dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			string categorylist = string.Empty;
			output.TagName = "ul";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "somecss");

			var content = await output.GetChildContentAsync();
			string format = content.GetContent();

			foreach (CategoryViewModel category in Categories)
			{
				categorylist = categorylist + string.Format(CATEGORY_ITEM_TEMPLATE, _dasBlogSettings.GetCategoryViewUrl(category.CategoryUrl), category.Category) + format;
			}

			output.Content.SetHtmlContent(categorylist);
		}
	}
}
