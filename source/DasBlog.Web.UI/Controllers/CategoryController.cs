using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using System.Diagnostics;
using System.Linq;
using EventCodes = DasBlog.Services.ActivityLogs.EventCodes;
using EventDataItem = DasBlog.Services.ActivityLogs.EventDataItem;

namespace DasBlog.Web.Controllers
{
	public class CategoryController : DasBlogBaseController
	{
		private readonly ICategoryManager categoryManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly ILogger<CategoryController> logger;
		private const string CATEGORY = "Category";

		public CategoryController(ICategoryManager categoryManager, IDasBlogSettings dasBlogSettings, IHttpContextAccessor httpContextAccessor,
									ILogger<CategoryController> logger)
			: base(dasBlogSettings)
		{
			this.categoryManager = categoryManager;
			this.dasBlogSettings = dasBlogSettings;
			this.httpContextAccessor = httpContextAccessor;
			this.logger = logger;
		}

		[HttpGet("category")]
		public IActionResult Category()
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var viewModel = GetCategoryListFromCategoryManager(string.Empty);

			stopWatch.Stop();
			logger.LogInformation(new EventDataItem(EventCodes.Site, null, $"CategoryController.Category Time elapsed: {stopWatch.Elapsed.TotalMilliseconds}ms"));

			return View(viewModel);
		}

		[HttpGet("category/{cat}")]
		public IActionResult Category(string cat)
		{
			var stopWatch = new Stopwatch();
			stopWatch.Start();

			var viewModel = GetCategoryListFromCategoryManager(cat.ToLower());

			stopWatch.Stop();
			logger.LogInformation(new EventDataItem(EventCodes.Site, null, $"CategoryController.Category ({cat}) Time elapsed: {stopWatch.Elapsed.TotalMilliseconds}ms"));

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

			var viewModel = CategoryListViewModel.Create(entryList, dasBlogSettings, categoryTile);

			DefaultPage(CATEGORY);
			return viewModel;
		}
	}
}
