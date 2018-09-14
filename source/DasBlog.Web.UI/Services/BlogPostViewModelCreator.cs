using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DasBlog.Web.Services
{
	public class BlogPostViewModelCreator : IBlogPostViewModelCreator
	{
		private IBlogManager blogManager;
		private IMapper mapper;

		public BlogPostViewModelCreator(IBlogManager blogManager, IMapper mapper)
		{
			this.blogManager = blogManager;
			this.mapper = mapper;
		}
		public PostViewModel CreateBlogPostVN()
		{
			PostViewModel post = new PostViewModel();
			post.IsPublic = true;
			post.Languages = GetAlllanguages();
//			post.AllCategories = mapper.Map<List<CategoryViewModel>>(blogManager.GetCategories());
			return post;
		}
		/// <summary>
		/// the universe of languagses based on CultureInfo for CultureType.AllCultures
		/// </summary>
		/// <returns>All available languages in native name order</returns>
		private IEnumerable<SelectListItem> GetAlllanguages()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			// setup temp store for listitem items, for sorting
			List<SelectListItem> cultureList = new List<SelectListItem>(cultures.Length);

			foreach (CultureInfo ci in cultures)
			{
				string langName = (ci.NativeName != ci.EnglishName) ? ci.NativeName + " / " + ci.EnglishName : ci.NativeName;

				if (langName.Length > 55)
				{
					langName = langName.Substring(0, 55) + "...";
				}

				if (string.IsNullOrEmpty(ci.Name))
				{
					langName = string.Empty;		// invariant language (invariant country)
				}

				cultureList.Add(new SelectListItem{ Value = ci.Name, Text = langName});
			}
			CultureInfo sortCulture = CultureInfo.CurrentCulture;

			// sort the list
			cultureList.Sort(delegate(SelectListItem x, SelectListItem y)
			{
				// actual comparison
				return String.Compare(x.Text, y.Text, true, sortCulture);
			});
			// add to the languages listbox

			SelectListItem[] cultureListItems = cultureList.ToArray();

			return cultureListItems;
		}
	}
}
