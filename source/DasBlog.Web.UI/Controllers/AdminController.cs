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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

		public AdminController(IDasBlogSettings dasBlogSettings, IFileSystemBinaryManager fileSystemBinaryManager, IMapper mapper,
								IBlogManager blogManager, IHostApplicationLifetime appLifetime, ILogger<AdminController> logger) : base(dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.fileSystemBinaryManager = fileSystemBinaryManager;
			this.mapper = mapper;
			this.blogManager = blogManager;
			this.appLifetime = appLifetime;
			this.logger = logger;
		}

		[HttpGet]
		[Route("/admin")]
		[Route("/admin/settings")]
		public IActionResult Settings()
		{
			var dbsvm = new DasBlogSettingsViewModel();
			dbsvm.MetaConfig = mapper.Map<MetaViewModel>(dasBlogSettings.MetaTags);
			dbsvm.SiteConfig = mapper.Map<SiteViewModel>(dasBlogSettings.SiteConfiguration);
			dbsvm.Posts = blogManager.GetAllEntries()
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();

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
				return Settings(settings);
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
				return Settings(settings);
			}
			dasBlogSettings.SiteConfiguration = site;

			if (!fileSystemBinaryManager.SaveMetaConfig(meta))
			{
				ModelState.AddModelError("", "Unable to save Meta configuration file.");
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Unable to save Site Config file"));
				return Settings(settings);
			}
			dasBlogSettings.MetaTags = meta;

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

		public IActionResult TestEmail()
		{
			if (!blogManager.SendTestEmail())
			{
				ModelState.AddModelError("", "Unable to save Site configuration file.");
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Unable to send test email"));
			}

			return RedirectToAction("Settings");
		}
	}
}
