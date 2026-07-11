using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.Site;
using DasBlog.Web.Models;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Quartz.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DasBlog.Web.Controllers
{
	public class HomeController : DasBlogBaseController
	{
		private readonly IBlogManager blogManager;
		private readonly ICommentManager commentManager;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMapper mapper;
		private readonly ILogger<HomeController> logger;
		private readonly IMemoryCache memoryCache;
		private readonly IExternalEmbeddingHandler embeddingHandler;

		public HomeController(IBlogManager blogManager, ICommentManager commentManager, IDasBlogSettings dasBlogSettings, IMapper mapper, 
								ILogger<HomeController> logger, IMemoryCache memoryCache, IExternalEmbeddingHandler embeddingHandler) : base(dasBlogSettings)
		{
			this.blogManager = blogManager;
			this.commentManager = commentManager;
			this.dasBlogSettings = dasBlogSettings;
			this.mapper = mapper;
			this.logger = logger;
			this.memoryCache = memoryCache;
			this.embeddingHandler = embeddingHandler;
		}

		public IActionResult Index()
		{
			if (!memoryCache.TryGetValue(CACHEKEY_FRONTPAGE, out ListPostsViewModel lpvm))
			{
			lpvm = new ListPostsViewModel
			{
				Posts = HomePagePosts()
			};

			foreach( var post in lpvm.Posts )
			{
				post.Content = embeddingHandler.InjectCategoryLinksAsync(post.Content).GetAwaiter().GetResult();
				post.Content = embeddingHandler.InjectIconsForBareLinksAsync(post.Content).GetAwaiter().GetResult();
			}

			AddComments(lpvm);

			if (dasBlogSettings.SiteConfiguration.EnableStartPageCaching)
				{
					memoryCache.Set(CACHEKEY_FRONTPAGE, lpvm, SiteCacheSettings());
				}
			}

			ViewData[Constants.ShowPageControl] = true;			
			ViewData[Constants.PageNumber] = 0;
			ViewData[Constants.PostCount] = lpvm.Posts.Count;
			ViewData[Constants.EnableProgressiveFrontPageLoading] = dasBlogSettings.SiteConfiguration.EnableProgressiveFrontPageLoading;

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

			var lpvm = PagePosts(index);

			AddComments(lpvm);

			ViewData["Message"] = string.Format("Page...{0}", index);
			ViewData[Constants.ShowPageControl] = true;			
			ViewData[Constants.PageNumber] = index;
			ViewData[Constants.PostCount] = lpvm.Posts.Count;

			return AggregatePostView(lpvm);
		}

		[HttpGet("page/{index:int}/posts")]
		public IActionResult PagePostsPartial(int index)
		{
			if (index < 1)
			{
				return BadRequest();
			}

			var lpvm = PagePosts(index);

			if (lpvm.Posts.Count == 0)
			{
				return NoContent();
			}

			AddComments(lpvm);

			return PartialView(dasBlogSettings.SiteConfiguration.ShowItemSummaryInAggregatedViews ? "_BlogItemsSummary" : "_BlogItems", lpvm);
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
					Comments = commentManager.GetComments(post.EntryId, false)
									.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
					PostId = post.EntryId,
					PostDate = post.CreatedDateTime,
					CommentUrl = dasBlogSettings.GetCommentViewUrl(post.PermaLink),
					AllowComments = post.AllowComments
				};
				post.Comments = lcvm;
			}

			return listPostsViewModel;
		}

		private ListPostsViewModel PagePosts(int index)
		{
			return new ListPostsViewModel
			{
				Posts = blogManager.GetEntriesForPage(index, Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList()
			};
		}

		private IList<PostViewModel> HomePagePosts()
		{
			IList<PostViewModel> posts = new List<PostViewModel>();

			if (!dasBlogSettings.SiteConfiguration.PostPinnedToHomePage.IsNullOrWhiteSpace() &&
				Guid.TryParse(dasBlogSettings.SiteConfiguration.PostPinnedToHomePage, out var results))
			{
				var entry = blogManager.GetBlogPostByGuid(results);

				if (entry != null)
				{
					posts.Add(mapper.Map<PostViewModel>(entry));
				}
			}
			else
			{
				posts = blogManager.GetFrontPagePosts(Request.Headers["Accept-Language"])
							.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();
			}

			return posts;
		}
	}
}

