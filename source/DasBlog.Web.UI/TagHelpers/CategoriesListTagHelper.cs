using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers
{
	public class CategoriesListTagHelper : TagHelper
	{
		public IList<CategoryViewModel> Categories { get; set; }

		private readonly IDasBlogSettings dasBlogSettings;
		private const string CATEGORY_ITEM_TEMPLATE = "<li><a href='{0}'>{1}</a></li>";

		public CategoriesListTagHelper(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			string categorylist = string.Empty;
			output.TagName = "ul";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "dasblog-ul-category");

			var content = await output.GetChildContentAsync();
			string format = content.GetContent();

			foreach (CategoryViewModel category in Categories)
			{
				categorylist = categorylist + string.Format(CATEGORY_ITEM_TEMPLATE, dasBlogSettings.GetCategoryViewUrl(category.CategoryUrl), category.Category) + format;
			}

			output.Content.SetHtmlContent(categorylist);
		}
	}
}
