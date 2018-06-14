using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Http;
using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using AutoMapper;
using DasBlog.Web.Settings;

namespace DasBlog.Web.Controllers
{
    public class CategoryController : DasBlogBaseController
	{
        private ICategoryManager _categoryManager;
        private IHttpContextAccessor _httpContextAccessor;
		private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public CategoryController(ICategoryManager categoryManager, IDasBlogSettings settings, 
			IHttpContextAccessor httpContextAccessor, IMapper mapper) : base(settings)
		{
            _categoryManager = categoryManager;
			_dasBlogSettings = settings;
			_httpContextAccessor = httpContextAccessor;
			_mapper = mapper;
		}

		[HttpGet("category/{cat}")]
		public IActionResult Category(string cat)
        {
            string languageFilter = _httpContextAccessor.HttpContext.Request.Headers["Accept-Language"];

			ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts =_categoryManager.GetEntries(cat, languageFilter)
				.Select(entry => _mapper.Map<PostViewModel>(entry)).ToList(); ;

			return ThemedView("Page", lpvm);
		}
	}
}
