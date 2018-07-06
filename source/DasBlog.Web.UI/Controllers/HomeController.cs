﻿using System.Diagnostics;
using System.Linq;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Controllers
{
	public class HomeController : DasBlogBaseController
	{
		private readonly IBlogManager _blogManager;
		private readonly IXmlRpcManager _xmlRpcManager;
		private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public HomeController(IBlogManager blogManager, IDasBlogSettings settings, IXmlRpcManager rpcManager, IMapper mapper) : base(settings)
		{
			_blogManager = blogManager;
			_xmlRpcManager = rpcManager;
			_dasBlogSettings = settings;
			_mapper = mapper;
		}

		public IActionResult Index()
		{
			ListPostsViewModel lpvm = new ListPostsViewModel();
			lpvm.Posts = _blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"])
							.Select(entry => _mapper.Map<PostViewModel>(entry)).ToList();
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
			lpvm.Posts = _blogManager.GetEntriesForPage(index, Request.Headers["Accept-Language"])
								.Select(entry => _mapper.Map<PostViewModel>(entry)).ToList();

			DefaultPage();

			return View("Page", lpvm);
		}

		[HttpGet("blogger")]
		public ActionResult Blogger()
		{
			// https://www.poppastring.com/blog/blogger.aspx
			// Implementation of Blogger XML-RPC Api
			// blogger
			// metaWebLog
			// mt

			return NoContent();
		}

		[Produces("text/xml")]
		[HttpPost("blogger")]
		public IActionResult Blogger([FromBody] string xmlrpcpost)
		{
			string blogger = _xmlRpcManager.Invoke(HttpContext.Request.Body);

			return Content(blogger);
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
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}

