using AutoMapper;
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

namespace DasBlog.Web.Controllers
{
	[Route("archive")]
	[ResponseCache(Duration = 14400, Location = ResponseCacheLocation.Any)]
	public class ArchiveController : DasBlogBaseController
	{
		private IArchiveManager archiveManager;
		private IHttpContextAccessor httpContextAccessor;
		private readonly IMapper mapper;
		private const string ARCHIVE = "Archive";

		public ArchiveController(IArchiveManager archiveManager, IHttpContextAccessor httpContextAccessor, IMapper mapper, 
									IDasBlogSettings settings) : base(settings)
		{
			this.archiveManager = archiveManager;
			this.httpContextAccessor = httpContextAccessor;
			this.mapper = mapper;
		}

		[HttpGet("")]
		public IActionResult Archive()
		{
			return Archive(DateTime.Now.Year, DateTime.Now.Month);
		}

		[HttpGet("{year}")]
		public IActionResult Archive(int year)
		{
			var dateTime = new DateTime(year, 1, 1);
			var months = GetMonthsViewModel(dateTime, true);
			return View(months);
		}

		[HttpGet("{year}/{month}")]
		public IActionResult Archive(int year, int month)
		{
			DateTime dateTime = new DateTime(year, month, 1);
			var months = GetMonthsViewModel(dateTime);
			return View(months);
		}

		[HttpGet("{year}/{month}/{day}")]
		public IActionResult Archive(int year, int month, int day)
		{
			var dateTime = new DateTime(year, month, day);
			var months = GetMonthsViewModel(dateTime);
			return View(months);
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

			//TODO: Do I need this?
			//entries = new EntryCollection(entries.OrderBy(e => e.CreatedUtc));

			DefaultPage(ARCHIVE);
			return MonthViewViewModel.Create(dateTime, entries, mapper);
		}
	}
}
