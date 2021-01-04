using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
			List<CommentAdminViewModel> comments = null;

			if (postid != null)
			{
				comments = blogManager.GetComments(postid, true).Select(comment => mapper.Map<CommentAdminViewModel>(comment)).ToList();
			}
			else
			{
				comments = blogManager.GetAllComments().Select(comment => mapper.Map<CommentAdminViewModel>(comment)).ToList();
			}

			foreach (var cmt in comments)
			{
				cmt.Title = blogManager.GetBlogPostByGuid(new Guid(cmt.BlogPostId))?.Title;
			}

			return View(comments.OrderBy(d => d.Date).ToList());
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
