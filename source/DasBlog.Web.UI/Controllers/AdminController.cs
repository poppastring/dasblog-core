using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Models.AdminViewModels;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Markdig;
using NBR = newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class AdminController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IFileSystemBinaryManager fileSystemBinaryManager;
		private readonly IMapper mapper;
		private readonly IBlogManager blogManager;
		private readonly IHostApplicationLifetime appLifetime;
		private readonly ILogger<AdminController> logger;
		private readonly IMemoryCache memoryCache;
		private readonly List<PostViewModel> posts = [];	

		public AdminController(IDasBlogSettings dasBlogSettings, IFileSystemBinaryManager fileSystemBinaryManager, IMapper mapper,
								IBlogManager blogManager, IHostApplicationLifetime appLifetime, ILogger<AdminController> logger, IMemoryCache memoryCache) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.fileSystemBinaryManager = fileSystemBinaryManager;
			this.mapper = mapper;
			this.blogManager = blogManager;
			this.appLifetime = appLifetime;
			this.logger = logger;
			this.memoryCache = memoryCache;
			this.posts = blogManager.GetAllEntries()
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();
		}

		[HttpGet]
		[Route("/admin")]
		[Route("/admin/settings")]
		public IActionResult Settings()
		{
			var dbsvm = new DasBlogSettingsViewModel();
			dbsvm.MetaConfig = mapper.Map<MetaViewModel>(dasBlogSettings.MetaTags);
			dbsvm.SiteConfig = mapper.Map<SiteViewModel>(dasBlogSettings.SiteConfiguration);
			dbsvm.Posts = posts;

			return View(dbsvm);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/settings")]
		public IActionResult Settings(DasBlogSettingsViewModel settings)
		{
			//save settings and reload...
			if (ModelState.ErrorCount > 0)
			{
				settings.Posts = posts;
				return View("Settings", settings);
			}

			var site = mapper.Map<SiteConfig>(settings.SiteConfig);
			var meta = mapper.Map<MetaTags>(settings.MetaConfig);

			site.SpamBlockingService = dasBlogSettings.SiteConfiguration.SpamBlockingService;
			site.CrosspostSites = dasBlogSettings.SiteConfiguration.CrosspostSites;
			site.PingServices = dasBlogSettings.SiteConfiguration.PingServices;

			if (!fileSystemBinaryManager.SaveSiteConfig(site))
			{
				ModelState.AddModelError("", "Unable to save Site configuration file.");
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Unable to save Site Config file"));
				settings.Posts = posts;
				return View("Settings", settings);
			}
			dasBlogSettings.SiteConfiguration = site;

			if (!fileSystemBinaryManager.SaveMetaConfig(meta))
			{
				ModelState.AddModelError("", "Unable to save Meta configuration file.");
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Unable to save Meta Config file"));
				settings.Posts = posts;
				return View("Settings", settings);
			}
			dasBlogSettings.MetaTags = meta;

			logger.LogInformation(new EventDataItem(EventCodes.Site, null, "Site settings updated"));

			TempData["SuccessMessage"] = "Settings saved successfully!";
			return Settings();
		}

		[HttpGet]
		[Route("/admin/manage-comments")]
		[HttpGet("/admin/manage-comments/{postid}")]
		public IActionResult ManageComments(string postid)
		{
			var comments = new List<CommentAdminViewModel>();

			if (postid != null)
			{
				comments = blogManager.GetComments(postid, true).Select(comment => mapper.Map<CommentAdminViewModel>(comment)).ToList();
				ViewData[Constants.CommentShowPageControl] = false;
			}
			else
			{
				comments = blogManager.GetCommentsFrontPage().Select(comment => mapper.Map<CommentAdminViewModel>(comment)).ToList();
				ViewData[Constants.CommentShowPageControl] = true;
			}

			foreach (var cmt in comments)
			{
				cmt.Title = blogManager.GetBlogPostByGuid(new Guid(cmt.BlogPostId))?.Title;
			}

			ViewData[Constants.CommentPageNumber] = 0;
			ViewData[Constants.CommentPostCount] = 5;
			return View(comments.OrderByDescending(d => d.Date).ToList());
		}

		[HttpGet]
		[Route("/admin/manage-comments/page")]
		[HttpGet("/admin/manage-comments/page/{page}")]
		public IActionResult ManageCommentsByPage(int page) 
		{
			var comments = new List<CommentAdminViewModel>();

			if (page > 0)
			{
				comments = blogManager.GetCommentsForPage(page).Select(comment => mapper.Map<CommentAdminViewModel>(comment)).ToList();
			}
			else
			{
				comments = blogManager.GetCommentsFrontPage().Select(comment => mapper.Map<CommentAdminViewModel>(comment)).ToList();
			}

			foreach (var cmt in comments)
			{
				cmt.Title = blogManager.GetBlogPostByGuid(new Guid(cmt.BlogPostId))?.Title;
			}

			ViewData[Constants.CommentShowPageControl] = true;
			ViewData[Constants.CommentPostCount] = 5;
			ViewData[Constants.CommentPageNumber] = page;

			return View("ManageComments", comments.OrderByDescending(d => d.Date).ToList());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Route("/admin/add-admin-comment")]
		public IActionResult AddAdminComment(AddCommentViewModel addcomment)
		{
			var errors = new List<string>();

			if (dasBlogSettings.SiteConfiguration.AllowMarkdownInComments)
			{
				var pipeline = new MarkdownPipelineBuilder().UseReferralLinks("nofollow").Build();
				addcomment.Content = Markdown.ToHtml(addcomment.Content, pipeline);
			}

			var commt = mapper.Map<NBR.Comment>(addcomment);
			commt.AuthorIPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
			commt.AuthorUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
			commt.EntryId = Guid.NewGuid().ToString();
			commt.SpamState = NBR.SpamState.NotSpam; // always good from the admin
			commt.IsPublic = true; // Admin comments are always public
			commt.CreatedUtc = commt.ModifiedUtc = DateTime.UtcNow;

			var state = blogManager.AddComment(addcomment.TargetEntryId, commt);

			if (state == NBR.CommentSaveState.Failed)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Failed to save admin comment: {0}", commt.TargetTitle));
				errors.Add("Failed to save comment.");
			}

			if (state == NBR.CommentSaveState.SiteCommentsDisabled)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Comments are closed for this post: {0}", commt.TargetTitle));
				errors.Add("Comments are closed for this post.");
			}

			if (state == NBR.CommentSaveState.PostCommentsDisabled)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Comments are currently disabled: {0}", commt.TargetTitle));
				errors.Add("Comments are currently disabled.");
			}

			if (state == NBR.CommentSaveState.NotFound)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Invalid Post Id: {0}", commt.TargetTitle));
				errors.Add("Invalid Post Id.");
			}

			if (errors.Count > 0)
			{
				TempData["ErrorMessage"] = string.Join(" ", errors);
				return RedirectToAction("ManageComments");
			}

			logger.LogInformation(new EventDataItem(EventCodes.CommentAdded, null, "Admin comment created on: {0}", commt.TargetTitle));
			BreakSiteCache();

			TempData["SuccessMessage"] = "Comment added successfully!";
			return RedirectToAction("ManageComments");
		}

		private void BreakSiteCache()
		{
			memoryCache.Remove(CACHEKEY_RSS);
			memoryCache.Remove(CACHEKEY_FRONTPAGE);
			memoryCache.Remove(CACHEKEY_ARCHIVE);
		}
	}
}
