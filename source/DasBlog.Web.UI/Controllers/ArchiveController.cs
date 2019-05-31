using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DasBlog.Web.Controllers
{
	[Route("archive")]
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
			var months = GetMonthsViewModel(dateTime);
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
			DateTime dateTime = new DateTime(year, month, day);
			var months = GetMonthsViewModel(dateTime);
			return View(months);
		}

		private MonthViewViewModel GetMonthsViewModel(DateTime dateTime)
		{
			string languageFilter = httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

			ViewBag.PreviousMonth = dateTime.AddMonths(-1).Date;
			ViewBag.NextMonth = dateTime.AddMonths(1).Date;
			ViewBag.CurrentMonth = dateTime.Date;
			var entries = archiveManager.GetEntriesForMonth(dateTime, languageFilter);

			DefaultPage(ARCHIVE);
			return MonthViewViewModel.Create(dateTime, entries, mapper);
		}
	}
}
