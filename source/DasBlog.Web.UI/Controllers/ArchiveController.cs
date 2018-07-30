using System;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	[Route("archive")]
	public class ArchiveController : DasBlogController
	{
		private IArchiveManager _archiveManager;
		private IHttpContextAccessor _httpContextAccessor;
		private readonly IMapper mapper;

		public ArchiveController(IArchiveManager archiveManager, IHttpContextAccessor httpContextAccessor, IMapper mapper)
		{
			_archiveManager = archiveManager;
			_httpContextAccessor = httpContextAccessor;
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
			DateTime dateTime = new DateTime(year, 1, 1);
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
			string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

			ViewBag.PreviousMonth = dateTime.AddMonths(-1).Date;
			ViewBag.NextMonth = dateTime.AddMonths(1).Date;
			ViewBag.CurrentMonth = dateTime.Date;
			var entries = _archiveManager
				.GetEntriesForMonth(dateTime, languageFilter);
			return MonthViewViewModel.Create(dateTime, entries, mapper);
		}
	}
}
