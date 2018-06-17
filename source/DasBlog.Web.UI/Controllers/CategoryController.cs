using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	public class CategoryController : DasBlogBaseController
	{
		private readonly ICategoryManager _categoryManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryManager categoryManager, IDasBlogSettings settings, IHttpContextAccessor httpContextAccessor, IMapper mapper)
			: base(settings)
		{
			_categoryManager = categoryManager;
			_dasBlogSettings = settings;
			_httpContextAccessor = httpContextAccessor;
			_mapper = mapper;
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
				? _categoryManager.GetEntries(category, _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"])
				: _categoryManager.GetEntries();

			var viewModel = CategoryListViewModel.Create(entries);
			return viewModel;
		}
	}
}
