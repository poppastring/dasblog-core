using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using System.Linq;

namespace DasBlog.Web.Controllers
{
	public class CategoryController : DasBlogBaseController
	{
		private readonly ICategoryManager categoryManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IDasBlogSettings dasBlogSettings;
		private const string CATEGORY = "Category";

		public CategoryController(ICategoryManager categoryManager, IDasBlogSettings dasBlogSettings, IHttpContextAccessor httpContextAccessor)
			: base(dasBlogSettings)
		{
			this.categoryManager = categoryManager;
			this.dasBlogSettings = dasBlogSettings;
			this.httpContextAccessor = httpContextAccessor;
		}

		[HttpGet("category")]
		public IActionResult Category()
		{
			var viewModel = GetCategoryListFromCategoryManager(string.Empty);
			return View(viewModel);
		}

		[HttpGet("category/{cat}")]
		public IActionResult Category(string cat)
		{
			var viewModel = GetCategoryListFromCategoryManager(cat.ToLower());
			return View(viewModel);
		}

		private CategoryListViewModel GetCategoryListFromCategoryManager(string category)
		{
			category = category.ToLower();

			var entries = !string.IsNullOrEmpty(category)
				? categoryManager.GetEntries(category, httpContextAccessor.HttpContext.Request.Headers["Accept-Language"])
				: categoryManager.GetEntries();

			var entryList = new EntryCollection();
			foreach (var entry in entries?.ToList()?.Where(e => e.IsPublic))
			{
				entryList.Add(entry);
			}

			var categoryTile = categoryManager.GetCategoryTitle(category);

			var viewModel = CategoryListViewModel.Create(entryList, categoryTile);

			DefaultPage(CATEGORY);
			return viewModel;
		}
	}
}
