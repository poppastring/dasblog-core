using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Http;
using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using AutoMapper;

namespace DasBlog.Web.Controllers
{
    public class CategoryController : Controller
    {
        private ICategoryManager _categoryManager;
        private IHttpContextAccessor _httpContextAccessor;
		private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryManager categoryManager, IDasBlogSettings settings, 
			IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _categoryManager = categoryManager;
			_dasBlogSettings = settings;
			_httpContextAccessor = httpContextAccessor;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("category/{cat}")]
		public IActionResult Category(string cat)
        {
            string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

			ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts =_categoryManager.GetEntries(cat, languageFilter)
				.Select(entry => _mapper.Map<PostViewModel>(entry)).ToList(); ;

			return ThemedView("Page", lpvm);
		}

		//TODO: Maybe a helper or base class?
		private ViewResult ThemedView(string v, ListPostsViewModel lpvm)
		{
			return View(string.Format("/Themes/{0}/{1}.cshtml",
						_dasBlogSettings.SiteConfiguration.Theme, v), lpvm);
		}

		//TODO: Maybe a helper or base class?
		private void DefaultPage()
		{
			ViewData["Title"] = _dasBlogSettings.SiteConfiguration.Title;
			ViewData["Description"] = _dasBlogSettings.SiteConfiguration.Description;
			ViewData["Keywords"] = _dasBlogSettings.MetaTags.MetaKeywords;
			ViewData["Canonical"] = _dasBlogSettings.SiteConfiguration.Root;
			ViewData["Author"] = _dasBlogSettings.SiteConfiguration.Copyright;
		}
	}
}
