using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;


namespace DasBlog.Web.Controllers
{
	public class HomeController : DasBlogBaseController
	{
		private readonly IBlogManager blogManager;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMapper mapper;
		private readonly ILogger<HomeController> logger;
		private readonly IMemoryCache memoryCache;

		public HomeController(IBlogManager blogManager, IDasBlogSettings settings, IXmlRpcManager rpcManager, 
							IMapper mapper, ILogger<HomeController> logger, IMemoryCache memoryCache) : base(settings)
		{
			this.blogManager = blogManager;
			dasBlogSettings = settings;
			this.mapper = mapper;
			this.logger = logger;
			this.memoryCache = memoryCache;
		}

		public IActionResult Index()
		{
			if (!memoryCache.TryGetValue(CACHEKEY_FRONTPAGE, out ListPostsViewModel lpvm))
			{
				lpvm = new ListPostsViewModel
				{
					Posts = blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry))
								.Select(editentry => editentry).ToList()
				};

				AddComments(lpvm);

				memoryCache.Set(CACHEKEY_FRONTPAGE, lpvm, SiteCacheSettings());

				logger.LogDebug($"In Index - {lpvm.Posts.Count} post found");
			}

			ViewData[Constants.ShowPageControl] = true;			
			ViewData[Constants.PageNumber] = 0;
			ViewData[Constants.PostCount] = lpvm.Posts.Count;

			return AggregatePostView(lpvm);
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


			var lpvm = new ListPostsViewModel
			{
				Posts = blogManager.GetEntriesForPage(index, Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList()
			};

			AddComments(lpvm);

			ViewData["Message"] = string.Format("Page...{0}", index);
			ViewData[Constants.ShowPageControl] = true;			
			ViewData[Constants.PageNumber] = index;
			ViewData[Constants.PostCount] = lpvm.Posts.Count;

			return AggregatePostView(lpvm);
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
					var path = feature.Path;
					var ex = feature.Error;
				}
				return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message, null);
				return Content("DasBlog - an error occurred (and reporting gailed) - Click the browser 'Back' button to try using the application");
			}
		}

		private ListPostsViewModel AddComments(ListPostsViewModel listPostsViewModel)
		{
			foreach (var post in listPostsViewModel.Posts)
			{
				var lcvm = new ListCommentsViewModel
				{
					Comments = blogManager.GetComments(post.EntryId, false)
					.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
					PostId = post.EntryId,
					PostDate = post.CreatedDateTime,
					CommentUrl = dasBlogSettings.GetCommentViewUrl(post.PermaLink)
				};
				post.Comments = lcvm;
			}

			return listPostsViewModel;
		}
	}
}

