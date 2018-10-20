using AutoMapper;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
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
		private readonly IMapper mapper;
		private const string CATEGORY = "Category";

		public CategoryController(ICategoryManager categoryManager, IDasBlogSettings settings, IHttpContextAccessor httpContextAccessor, IMapper mapper)
			: base(settings)
		{
			this.categoryManager = categoryManager;
			dasBlogSettings = settings;
			this.httpContextAccessor = httpContextAccessor;
			this.mapper = mapper;
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
			var viewModel = GetCategoryListFromCategoryManager(cat);
			return View(viewModel);
		}

		private CategoryListViewModel GetCategoryListFromCategoryManager(string category)
		{
			var entries = !string.IsNullOrEmpty(category)
				? categoryManager.GetEntries(category, httpContextAccessor.HttpContext.Request.Headers["Accept-Language"])
				: categoryManager.GetEntries();

			var entryList = new EntryCollection();
			foreach (var entry in entries?.ToList()?.Where(e => e.IsPublic))
			{
				entryList.Add(entry);
			}

			var viewModel = CategoryListViewModel.Create(entryList, category);

			DefaultPage(CATEGORY);
			return viewModel;
		}
	}
}
