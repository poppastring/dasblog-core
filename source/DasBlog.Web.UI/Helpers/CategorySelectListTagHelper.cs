using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Web.Core;
using DasBlog.Web.Repositories.Interfaces;
using DasBlog.Web.UI.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.UI.Helpers
{
	public class CategorySelectListTagHelper : TagHelper
	{
		public IList<CategoryViewModel> Categories { get; set; }
		private readonly IBlogRepository _blogRepository;
		private readonly IMapper _mapper;
		private const string CATEGORY_CHKBOX_TEMPLATE = "<label><input type='checkbox' name='{0}' value='{1}' {2}>{0}</label><br/>";
		private const string CHKBOX_CHECKED = "checked";

		public CategorySelectListTagHelper(IBlogRepository blogRepository, IMapper mapper)
		{
			_blogRepository = blogRepository;
			_mapper = mapper;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			string checkboxlist = string.Empty;
			List<CategoryViewModel> allcategories = _mapper.Map<List<CategoryViewModel>>(_blogRepository.GetCategories());

			output.TagName = "div";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("class", "catselect");

			foreach (CategoryViewModel category in allcategories)
			{
				string checkedVal = string.Empty;
				if (Categories.Count(x => x.Category == category.Category) > 0)
				{
					checkedVal = CHKBOX_CHECKED;
				}

				checkboxlist = checkboxlist + string.Format(CATEGORY_CHKBOX_TEMPLATE, category.Category, category.Category, checkedVal);
			}

			output.Content.SetHtmlContent(checkboxlist);
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}
	}
}
