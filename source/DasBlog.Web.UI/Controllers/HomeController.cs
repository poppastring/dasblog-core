using System;
using System.Diagnostics;
using System.Linq;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	public class HomeController : DasBlogBaseController
	{
		private readonly IBlogManager blogManager;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMapper mapper;
		private readonly ILogger<HomeController> logger;
		
		public HomeController(IBlogManager blogManager, IDasBlogSettings settings, 
			IXmlRpcManager rpcManager, IMapper mapper, ILogger<HomeController> logger) : base(settings)
		{
			this.blogManager = blogManager;
			dasBlogSettings = settings;
			this.mapper = mapper;
			this.logger = logger;
		}

		public IActionResult Index()
		{
			ListPostsViewModel lpvm = new ListPostsViewModel();
			lpvm.Posts = blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"])
							.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();
			logger.LogDebug($"In Index - {lpvm.Posts.Count} post found");
			DefaultPage();

			return View("Page", lpvm);
		}

		[HttpGet("page")]
		public IActionResult Page()
		{
			return Index();
		}

		[HttpGet("page/{index:int}")]
		public IActionResult Page(int index)
		{
			if (index == 0)
			{
				return Index();
			}

			ViewData["Message"] = string.Format("Page...{0}", index);

			ListPostsViewModel lpvm = new ListPostsViewModel();
			lpvm.Posts = blogManager.GetEntriesForPage(index, Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();

			DefaultPage();

			return View("Page", lpvm);
		}

		public IActionResult About()
		{
			DefaultPage();

			ViewData["Message"] = "Your application description page.";

			return NoContent();
		}

		public IActionResult Contact()
		{
			DefaultPage();

			ViewData["Message"] = "Your contact page.";

			return NoContent();
		}

		public IActionResult Error()
		{
			try
			{
				var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
				if (feature != null)
				{
					string path = feature.Path;
					Exception ex = feature.Error;
				}
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message, null);
				return Content(
					"DasBlog - an error occurred (and reporting gailed) - Click the browser 'Back' button to try using the application");
			}
		}
	}
}

