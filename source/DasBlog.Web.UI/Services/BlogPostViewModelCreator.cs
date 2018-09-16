using System;
using System.Collections.Generic;
using System.Globalization;
using AutoMapper;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using NodaTime;

namespace DasBlog.Web.Services
{
	public class BlogPostViewModelCreator : IBlogPostViewModelCreator
	{
		private IBlogManager blogManager;
		private IMapper mapper;
		private ITimeZoneProvider timeZoneProvider;

		public BlogPostViewModelCreator(IBlogManager blogManager, IMapper mapper
		  ,ITimeZoneProvider timeZoneProvider)
		{
			this.blogManager = blogManager;
			this.mapper = mapper;
			this.timeZoneProvider = timeZoneProvider;
		}
		public PostViewModel CreateBlogPostVM()
		{
			PostViewModel post = new PostViewModel();
			var tz = timeZoneProvider.GetConfiguredTimeZone();
			var offset = tz.GetUtcOffset(new Instant());
			post.CreatedDateTime = DateTime.UtcNow.Add(new TimeSpan(0,0,0,offset.Seconds));
			post.IsPublic = true;
			post.Syndicated = true;
			post.AllowComments = true;
			post.Languages = GetAlllanguages();
			post.AllCategories = mapper.Map<List<CategoryViewModel>>(blogManager.GetCategories());
			return post;
		}

		public void AddAllLanguages(PostViewModel pvm)
		{
			pvm.Languages = GetAlllanguages();
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
