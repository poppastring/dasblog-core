using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.Controllers
{
    [Route("archive")]
    public class ArchiveController : Controller
    {
        private IArchiveRepository _archiveRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public ArchiveController(IArchiveRepository archiveRepository, IHttpContextAccessor httpContextAccessor)
        {
            _archiveRepository = archiveRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return Archive(DateTime.Now.Year);
        }

        [HttpGet("{year}")]
        public IActionResult Archive(int year)
        {
            DateTime dateTime = new DateTime(year, 1, 1);
            string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

            var months = _archiveRepository.GetEntriesForYear(dateTime, languageFilter);
            return View("", months);
        }

        [HttpGet("{year}/{month}")]
        public IActionResult Archive(int year, int month)
        {
            DateTime dateTime = new DateTime(year, month, 1);
            string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

            var months = _archiveRepository.GetEntriesForMonth(dateTime, languageFilter);
            return View("", months);
        }

        [HttpGet("{year}/{month}/{day}")]
        public IActionResult Archive(int year, int month, int day)
        {
            DateTime dateTime = new DateTime(year, month, day);
            string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

            var months = _archiveRepository.GetEntriesForMonth(dateTime, languageFilter);
            return View("", months);
        }
    }
}