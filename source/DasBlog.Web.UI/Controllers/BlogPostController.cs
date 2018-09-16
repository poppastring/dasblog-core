using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using DasBlog.Core.Common;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using newtelligence.DasBlog.Runtime;
using static DasBlog.Core.Common.Utils;
using Microsoft.Extensions.Logging;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	public class BlogPostController : DasBlogBaseController
	{
		private readonly IBlogManager blogManager;
		private readonly ICategoryManager categoryManager;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMapper mapper;
		private readonly IFileSystemBinaryManager binaryManager;
		private readonly ILogger<BlogPostController> logger;
		private readonly IBlogPostViewModelCreator modelViewCreator;

		public BlogPostController(IBlogManager blogManager, IHttpContextAccessor httpContextAccessor,
		  IDasBlogSettings settings, IMapper mapper, ICategoryManager categoryManager
		  ,IFileSystemBinaryManager binaryManager, ILogger<BlogPostController> logger
		  ,IBlogPostViewModelCreator modelViewCreator) : base(settings)
		{
			this.blogManager = blogManager;
			this.categoryManager = categoryManager;
			this.httpContextAccessor = httpContextAccessor;
			dasBlogSettings = settings;
			this.mapper = mapper;
			this.binaryManager = binaryManager;
			this.logger = logger;
			this.modelViewCreator = modelViewCreator;
		}

		[AllowAnonymous]
		public IActionResult Post(string posttitle, int day)
		{
			ListPostsViewModel lpvm = new ListPostsViewModel();
			RouteAffectedFunctions routeAffectedFunctions = new RouteAffectedFunctions(
			  dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique);

			if (!routeAffectedFunctions.IsValidDay(day))
			{
				return NotFound();
			}

			DateTime? dt = routeAffectedFunctions.ConvertDayToDate(day);
			
			if (routeAffectedFunctions.IsSpecificPostRequested(posttitle, day))
			{
				var entry = blogManager.GetBlogPost(posttitle.Replace(dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement, 
																				string.Empty), dt);
				if (entry != null)
				{
					lpvm.Posts = new List<PostViewModel>() { mapper.Map<PostViewModel>(entry) };

					SinglePost(lpvm.Posts.First());

					return View("Page", lpvm);
				}
				else
				{
					return NotFound();
				}
			}
			else
			{
				return RedirectToAction("index", "home");
			}
		}

		[AllowAnonymous]
		[HttpGet("post/{postid:guid}")]
		public IActionResult PostGuid(Guid postid)
		{
			var lpvm = new ListPostsViewModel();
			var entry = blogManager.GetBlogPost(postid.ToString(), null);
			if (entry != null)
			{
				lpvm.Posts = new List<PostViewModel>() { mapper.Map<PostViewModel>(entry) };

				SinglePost(lpvm.Posts.First());

				return View("Page", lpvm);
			}
			else
			{
				return NotFound();
			}
		}

		[HttpGet("post/{postid:guid}/edit")]
		public IActionResult EditPost(Guid postid)
		{
			PostViewModel pvm = new PostViewModel();

			if (!string.IsNullOrEmpty(postid.ToString()))
			{
				var entry = blogManager.GetEntryForEdit(postid.ToString());
				if (entry != null)
				{
					pvm = mapper.Map<PostViewModel>(entry);
					modelViewCreator.AddAllLanguages(pvm);
					List<CategoryViewModel> allcategories = mapper.Map<List<CategoryViewModel>>(blogManager.GetCategories());

					foreach (var cat in allcategories)
					{
						if (pvm.Categories.Count(x => x.Category == cat.Category) > 0)
						{
							cat.Checked = true;
						}
					}

					pvm.AllCategories = allcategories;

					return View(pvm);
				}
			}

			return NotFound();
		}

		[HttpPost("post/edit")]
		public IActionResult EditPost(PostViewModel post, string submit)
		{
			// languages does not get posted as part of form
			modelViewCreator.AddAllLanguages(post);
			if (submit == Constants.BlogPostAddCategoryAction)
			{
				return HandleNewCategory(post);
			}
			if (submit == Constants.UploadImageAction)
			{
				return HandleImageUpload(post);
			}

			ValidatePost(post);
			if (!ModelState.IsValid)
			{
				return View(post);
			}

			if (!string.IsNullOrWhiteSpace(post.NewCategory))
			{
				ModelState.AddModelError(nameof(post.NewCategory)
				  , $"Please click 'Add' to add the category, \"{post.NewCategory}\" or clear the text before continuing");
				return View(post);
			}
			try
			{
				Entry entry = mapper.Map<Entry>(post);
				entry.Author = httpContextAccessor.HttpContext.User.Identity.Name;
				entry.Language = "en-us"; //TODO: We inject this fron http context?
				entry.Latitude = null;
				entry.Longitude = null;
				
				EntrySaveState sts = blogManager.UpdateEntry(entry);
				if (sts != EntrySaveState.Updated)
				{
					ModelState.AddModelError("", "Failed to edit blog post. Please check Logs for more details.");
					return View(post);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message, null);
				ModelState.AddModelError("", "Failed to edit blog post. Please check Logs for more details.");
			}

			return View(post);
		}

		[HttpGet("post/create")]
		public IActionResult CreatePost()
		{
			PostViewModel post = modelViewCreator.CreateBlogPostVM();
/*
			PostViewModel post = new PostViewModel();
			post.CreatedDateTime = DateTime.UtcNow;  //TODO: Set to the timezone configured???
			post.AllCategories = mapper.Map<List<CategoryViewModel>>(blogManager.GetCategories());
			post.Languages = GetAlllanguages();
*/

			return View(post);
		}

		[HttpPost("post/create")]
		public IActionResult CreatePost(PostViewModel post, string submit)
		{
			modelViewCreator.AddAllLanguages(post);
			if (submit == Constants.BlogPostAddCategoryAction)
			{
				return HandleNewCategory(post);
			}

			if (submit == Constants.UploadImageAction)
			{
				return HandleImageUpload(post);
			}

			ValidatePost(post);
			if (!ModelState.IsValid)
			{
				return View(post);
			}
			if (!string.IsNullOrWhiteSpace(post.NewCategory))
			{
				ModelState.AddModelError(nameof(post.NewCategory)
					, $"Please click 'Add' to add the category, \"{post.NewCategory}\" or clear the text before continuing");
				return View(post);
			}

			try
			{
				Entry entry = mapper.Map<Entry>(post);

				entry.Initialize();
				entry.Author = httpContextAccessor.HttpContext.User.Identity.Name;
				entry.Language = post.Language;
				entry.Latitude = null;
				entry.Longitude = null;

				EntrySaveState sts = blogManager.CreateEntry(entry);
				if (sts != EntrySaveState.Added)
				{
					ModelState.AddModelError("", "Failed to create blog post. Please check Logs for more details.");
					return View(post);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message, null);
				ModelState.AddModelError("", "Failed to edit blog post. Please check Logs for more details.");
			}

			return View("views/blogpost/editPost.cshtml", post);
		}

		[HttpGet("post/{postid:guid}/delete")]
		public IActionResult DeletePost(Guid postid)
		{
			try
			{
				blogManager.DeleteEntry(postid.ToString());
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message, null);
				RedirectToAction("Error");
			}

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		[HttpGet("post/{postid:guid}/comments")]
		[HttpGet("post/{postid:guid}/comments/{commentid:guid}")]
		public IActionResult Comment(Guid postid)
		{
			ListPostsViewModel lpvm = null;

			var entry = blogManager.GetBlogPost(postid.ToString(), null);

			if (entry != null)
			{
				lpvm = new ListPostsViewModel
				{
					Posts = new List<PostViewModel> { mapper.Map<PostViewModel>(entry) }
				};

				if (dasBlogSettings.SiteConfiguration.EnableComments)
				{
					var lcvm = new ListCommentsViewModel
					{
						Comments = blogManager.GetComments(postid.ToString(), false)
							.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
						PostId = postid.ToString(),
						PostDate = entry.CreatedUtc
					};

					lpvm.Posts.First().Comments = lcvm;
				}
			}

			SinglePost(lpvm?.Posts?.First());

			return View("page", lpvm);
		}

		[AllowAnonymous]
		[HttpPost("post/comments")]
		public IActionResult AddComment(AddCommentViewModel addcomment)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableComments)
			{
				return BadRequest();
			}

			if (!ModelState.IsValid)
			{
				Comment(new Guid(addcomment.TargetEntryId));
			}

			addcomment.Content = dasBlogSettings.FilterHtml(addcomment.Content);

			var commt = mapper.Map<Comment>(addcomment);
			commt.AuthorIPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
			commt.AuthorUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
			commt.CreatedUtc = commt.ModifiedUtc = DateTime.UtcNow;
			commt.EntryId = Guid.NewGuid().ToString();
			commt.IsPublic = !dasBlogSettings.SiteConfiguration.CommentsRequireApproval;

			var state = blogManager.AddComment(addcomment.TargetEntryId, commt);

			if (state == CommentSaveState.Failed)
			{
				ModelState.AddModelError("", "Comment failed");
				return StatusCode(500);
			}

			if (state == CommentSaveState.SiteCommentsDisabled)
			{
				ModelState.AddModelError("", "Comments are closed for this post");
				return StatusCode(403);
			}

			if (state == CommentSaveState.PostCommentsDisabled)
			{
				ModelState.AddModelError("", "Comment are currently disabled");
				return StatusCode(403);
			}

			if (state == CommentSaveState.NotFound)
			{
				ModelState.AddModelError("", "Invalid Target Post Id");
				return NotFound();
			}

			return Comment(new Guid(addcomment.TargetEntryId));
		}

		[HttpDelete("post/{postid:guid}/comments/{commentid:guid}")]
		public IActionResult DeleteComment(Guid postid, Guid commentid)
		{
			var state = blogManager.DeleteComment(postid.ToString(), commentid.ToString());

			if (state == CommentSaveState.Failed)
			{
				return StatusCode(500);
			}

			if (state == CommentSaveState.NotFound)
			{
				return NotFound();
			}

			return Ok();
		}

		[HttpPatch("post/{postid:guid}/comments/{commentid:guid}")]
		public IActionResult ApproveComment(Guid postid, Guid commentid)
		{
			var state = blogManager.ApproveComment(postid.ToString(), commentid.ToString());

			if (state == CommentSaveState.Failed)
			{
				return StatusCode(500);
			}

			if (state == CommentSaveState.NotFound)
			{
				return NotFound();
			}

			return Ok();
		}

		[AllowAnonymous]
		[HttpGet("post/category/{category}")]
		public IActionResult GetCategory(string category)
		{
			if (string.IsNullOrEmpty(category))
			{
				return RedirectToAction("Index", "Home");
			}

			ListPostsViewModel lpvm = new ListPostsViewModel();
			lpvm.Posts = categoryManager.GetEntries(category, httpContextAccessor.HttpContext.Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();

			DefaultPage();

			return View("Page", lpvm);
		}
		[AllowAnonymous]
		[HttpPost("/blogpost/search", Name=Constants.SearcherRouteName)]
		public IActionResult Search(string searchText)
		{
			ListPostsViewModel lpvm = new ListPostsViewModel();
			List<Entry> entries = blogManager.SearchEntries(WebUtility.HtmlEncode(searchText), Request.Headers["Accept-Language"]);
			if (entries != null)
			{
				lpvm.Posts = entries.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();

				return View("Page", lpvm);
			}

			return RedirectToAction("index", "home");
		}
		private IActionResult HandleNewCategory(PostViewModel post)
		{
			ModelState.ClearValidationState("");
			if (string.IsNullOrWhiteSpace(post.NewCategory))
			{
				ModelState.AddModelError(nameof(post.NewCategory)
				  ,"To add a category " +
				   "you must enter some text in the box next to the 'Add' button before clicking 'Add'");
				return View(post);
			}

			var newCategory = post.NewCategory?.Trim();
			var newCategoryDisplayName = EncodeCategoryUrl(newCategory, string.Empty );
						// Category names should not include special characters #200
			var newCategoryUrl = EncodeCategoryUrl(newCategory, string.Empty );
			if (post.AllCategories.Any(c => c.CategoryUrl == newCategoryUrl))
			{
				ModelState.AddModelError(nameof(post.NewCategory), $"The category, {post.NewCategory}, already exists");
			}
			else
			{
				post.AllCategories.Add(
				  new CategoryViewModel {
				  Category = newCategoryDisplayName
				  , CategoryUrl = newCategoryUrl, Checked = true});
				post.NewCategory = "";
				ModelState.Remove(nameof(post.NewCategory));	// ensure response refreshes page with view model's value
			}

			return View(post);
		}

		private IActionResult HandleImageUpload(PostViewModel post)
		{
			ModelState.ClearValidationState("");
			String fileName = post.Image?.FileName;
			if (string.IsNullOrEmpty(fileName))
			{
				ModelState.AddModelError(nameof(post.Image)
					, $"You must select a file before clicking \"{Constants.UploadImageAction}\" to upload it");
				return View(post);
			}

			string relativePath = null;
			try
			{
				using (var s = post.Image.OpenReadStream())
				{
					relativePath = binaryManager.SaveFile(s, fileName);
				}
			}
			catch (Exception e)
			{
				ModelState.AddModelError(nameof(post.Image), $"An error occurred while uploading image ({e.Message})");
				return View(post);
			}

			if (string.IsNullOrEmpty(relativePath))
			{
				ModelState.AddModelError(nameof(post.Image)
					, "Failed to upload file - reason unknown");
				return View(post);
			}

			string linkText = String.Format("<p><img border=\"0\" src=\"{0}\"></p>",
				relativePath);
			post.Content += linkText;
			ModelState.Remove(nameof(post.Content)); // ensure that model change is included in response
			return View(post);
		}

		private void ValidatePost(PostViewModel post)
		{
			RouteAffectedFunctions routeAffectedFunctions = new RouteAffectedFunctions(
			  dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique);
			DateTime? dt = routeAffectedFunctions.SelectDate(post);
			Entry entry = blogManager.GetBlogPost(
			  post.Title.Replace(" ", string.Empty)
			  ,dt);
			if (entry != null)
			{
				ModelState.AddModelError(string.Empty, "A post with this title already exists.  Titles must be unique");
			}
		}

		private class RouteAffectedFunctions
		{
			const string DATE_FORMAT = "yyyyMMdd";

			enum RouteType
			{
				IncludesDay,
				PostTitleOnly
			}

			RouteType routeType;
			public RouteAffectedFunctions(bool includeDay)
			{
				this.routeType = includeDay ? RouteType.IncludesDay : RouteType.PostTitleOnly;
			}

			public Func<string, int, bool> IsSpecificPostRequested
			{
				get
				{
					IDictionary<RouteType, Func<string, int, bool>> isSpecificPostRequested = new Dictionary<RouteType, Func<string, int, bool>>
					{
						{RouteType.PostTitleOnly, (posttitle, day) => !string.IsNullOrEmpty(posttitle)},
						{RouteType.IncludesDay, (posttitle, day) => !string.IsNullOrEmpty(posttitle) && day != 0}
					};
					return isSpecificPostRequested[routeType];
				}
			}
			public Func<int, bool> IsValidDay
			{
				get
				{
					IDictionary<RouteType, Func<int, bool>> isValidDay = new Dictionary<RouteType, Func<int, bool>>
					{
						{RouteType.PostTitleOnly, day => true},
						{RouteType.IncludesDay, day => DateTime.TryParseExact(day.ToString(), DATE_FORMAT
							, null, DateTimeStyles.AdjustToUniversal, out _)}
					};
					return isValidDay[routeType];
				}
			}
			public Func<int, DateTime?> ConvertDayToDate
			{
				get
				{
					IDictionary<RouteType, Func<int, DateTime?>> convertDayToDate = new Dictionary<RouteType, Func<int, DateTime?>>
					{
						{RouteType.PostTitleOnly, day => null},
						{RouteType.IncludesDay, day => DateTime.ParseExact(day.ToString(), DATE_FORMAT
							, null, DateTimeStyles.AdjustToUniversal)}
					};
					return convertDayToDate[routeType];
				}
			}
			public Func<PostViewModel, DateTime?> SelectDate
			{
				get
				{
					IDictionary<RouteType, Func<PostViewModel, DateTime?>> convertDayToDate = new Dictionary<RouteType, Func<PostViewModel, DateTime?>>
					{
						{RouteType.PostTitleOnly, pvm => null},
						{RouteType.IncludesDay, pvm => pvm.CreatedDateTime}
					};
					return convertDayToDate[routeType];
				}
			}

		}
	}
}
