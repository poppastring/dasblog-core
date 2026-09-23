using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using System.Linq;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using EventCodes = DasBlog.Services.ActivityLogs.EventCodes;
using DasBlog.Services.ActivityLogs;
using Microsoft.Extensions.Caching.Memory;

namespace DasBlog.Web.Controllers
{
	[Route("archive")]
	public class ArchiveController : DasBlogBaseController
	{
		private readonly IArchiveManager archiveManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IMapper mapper;
		private readonly ILogger<ArchiveController> logger;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMemoryCache memoryCache;
		private const string ARCHIVE = "Archive";

		public ArchiveController(IArchiveManager archiveManager, IHttpContextAccessor httpContextAccessor, IMapper mapper,
									ILogger<ArchiveController> logger, IDasBlogSettings settings, IMemoryCache memoryCache) : base(settings)
		{
			this.dasBlogSettings = settings;
			this.memoryCache = memoryCache;
			this.archiveManager = archiveManager;
			this.httpContextAccessor = httpContextAccessor;
			this.mapper = mapper;
			this.logger = logger;
		}

		[HttpGet("")]
		[RequireBlogFeatures]
		public IActionResult Archive()
		{
			return Archive(DateTime.Now.Year, DateTime.Now.Month);
		}

		[HttpGet("{year:int:min(1):max(9999)}")]
		[RequireBlogFeatures]
		public IActionResult Archive(int year)
		{
			if (!TryCreateDate(year, 1, 1, out var dateTime))
				return NotFound();

			var months = GetMonthsViewModel(dateTime, true);
			return View(months);
		}

		[HttpGet("{year:int:min(1):max(9999)}/{month:int:min(1):max(12)}")]
		[RequireBlogFeatures]
		public IActionResult Archive(int year, int month)
		{
			if (!TryCreateDate(year, month, 1, out var dateTime))
				return NotFound();

			var months = GetMonthsViewModel(dateTime);
			return View(months);
		}

		[HttpGet("{year:int:min(1):max(9999)}/{month:int:min(1):max(12)}/{day:int:min(1):max(31)}")]
		[RequireBlogFeatures]
		public IActionResult Archive(int year, int month, int day)
		{
			if (!TryCreateDate(year, month, day, out var dateTime))
				return NotFound();

			var months = GetMonthsViewModel(dateTime);
			return View(months);
		}

		[HttpGet("all")]
		[RequireBlogFeatures]
		public IActionResult ArchiveAll()
		{
			DefaultPage(Constants.ArchiveAllPageTitle);

			if (!memoryCache.TryGetValue(CACHEKEY_ARCHIVE, out ArchiveListViewModel alvm))
			{
				var entries = new EntryCollection();
				var languageFilter = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];
				var listofyears = archiveManager.GetDaysWithEntries().Select(i => i.Year).Distinct();

				foreach (var year in listofyears)
				{
					entries.AddRange(
						archiveManager.GetEntriesForYear(new DateTime(year, 1, 1), languageFilter).OrderByDescending(x => x.CreatedUtc));
				}

				alvm = new ArchiveListViewModel();

				foreach (var i in entries.Select(entry => mapper.Map<PostViewModel>(entry)))
				{
					var index = i.CreatedDateTime.Year * 100 + i.CreatedDateTime.Month;

					if (alvm.MonthEntries.ContainsKey(index))
					{
						alvm.MonthEntries[index].Add(i);
					}
					else
					{
						var list = new List<PostViewModel>() { i };
						alvm.MonthEntries.Add(index, list);
					}
				}
				memoryCache.Set(CACHEKEY_ARCHIVE, alvm, SiteCacheSettings());
			}

			return View(alvm);
		}

		private static bool TryCreateDate(int year, int month, int day, out DateTime dateTime)
		{
			if (year is < 1 or > 9999 || month is < 1 or > 12 || day < 1 || day > DateTime.DaysInMonth(year, month))
			{
				dateTime = default;
				return false;
			}

			dateTime = new DateTime(year, month, day);
			return true;
		}

		private List<MonthViewViewModel> GetMonthsViewModel(DateTime dateTime, bool wholeYear = false)
		{
			string languageFilter = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

			ViewBag.PreviousMonth = dateTime.AddMonths(-1).Date;
			ViewBag.NextMonth = dateTime.AddMonths(1).Date;
			ViewBag.CurrentMonth = dateTime.Date;

			//unique list of years for the top of archives
			var daysWithEntries = archiveManager.GetDaysWithEntries();
			ViewBag.Years = daysWithEntries.Select(i => i.Year).Distinct();

			EntryCollection entries;
			if (wholeYear)
				entries = archiveManager.GetEntriesForYear(dateTime, languageFilter);
			else
				entries = archiveManager.GetEntriesForMonth(dateTime, languageFilter);


			DefaultPage(ARCHIVE);
			return MonthViewViewModel.Create(dateTime, entries, mapper);
		}
	}
}
