using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Web.Repositories.Interfaces;
using newtelligence.DasBlog.Runtime;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.UI.Controllers
{
    [Route("category")]
    public class CategoryController : Controller
    {
        private ICategoryRepository _categoryRepository;
        private IHttpContextAccessor _httpContextAccessor;

        public CategoryController(ICategoryRepository categoryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _categoryRepository = categoryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            EntryCollection entries = _categoryRepository.GetEntries();

            return View();
        }

        [HttpGet("{category}")]
        public IActionResult Index(string category)
        {
            string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

            EntryCollection entries = _categoryRepository.GetEntries(category, languageFilter);

            return View("Index.cshtml");
        }
    }
}
