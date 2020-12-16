using AutoMapper;
using DasBlog.Core.Common;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ActivityLogs;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services.Interfaces;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using NBR = newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using reCAPTCHA.AspNetCore;

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
		private readonly IMemoryCache memoryCache;
        private readonly IRecaptchaService recaptcha;


		public BlogPostController(IBlogManager blogManager, IHttpContextAccessor httpContextAccessor, IDasBlogSettings dasBlogSettings, 
									IMapper mapper, ICategoryManager categoryManager, IFileSystemBinaryManager binaryManager, ILogger<BlogPostController> logger,
									IBlogPostViewModelCreator modelViewCreator, IMemoryCache memoryCache,IRecaptchaService recaptcha) 
									: base(dasBlogSettings)
		{
			this.blogManager = blogManager;
			this.categoryManager = categoryManager;
			this.httpContextAccessor = httpContextAccessor;
			this.dasBlogSettings = dasBlogSettings;
			this.mapper = mapper;
			this.binaryManager = binaryManager;
			this.logger = logger;
			this.modelViewCreator = modelViewCreator;
			this.memoryCache = memoryCache;
            this.recaptcha = recaptcha;
		}

		[AllowAnonymous]
		public IActionResult Post(string posttitle, string day, string month, string year)
		{
			var lpvm = new ListPostsViewModel();

			var uniquelinkdate = ValidateUniquePostDate(year, month, day);

			var entry = blogManager.GetBlogPost(posttitle, uniquelinkdate);
			if (entry != null)
			{
				var pvm = mapper.Map<PostViewModel>(entry);

				var lcvm = new ListCommentsViewModel
				{
					Comments = blogManager.GetComments(entry.EntryId, false)
									.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
					PostId = entry.EntryId,
					PostDate = entry.CreatedUtc,
					CommentUrl = dasBlogSettings.GetCommentViewUrl(posttitle),
					ShowComments = dasBlogSettings.SiteConfiguration.ShowCommentsWhenViewingEntry,
					AllowComments = entry.AllowComments
				};
				pvm.Comments = lcvm;

				if (!dasBlogSettings.SiteConfiguration.UseAspxExtension && httpContextAccessor.HttpContext.Request.Path.Value.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
				{
					return RedirectPermanent(pvm.PermaLink);
				}

				lpvm.Posts = new List<PostViewModel>() { pvm };
				return SinglePostView(lpvm);
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
			var entry = blogManager.GetBlogPostByGuid(postid);
			if (entry != null)
			{

				/*
				 * Very old DasBlog links like
				 * /PermaLink.aspx?guid=b5790285-2eb7-4198-ac1d-6cfbf20735a4
				 * turn into 
				 * /post/b5790285-2eb7-4198-ac1d-6cfbf20735a4 
				 * (given correct IISrewrites)
				 * but fails to render because the Comments are never loaded, 
				 * so you'll get the post but it shows ZERO comments. 
				 * Better to just redirect to the right URL
				 * I can't figure how to redirect 302 correctly given a /blog baseURL, so for now at least it doesn't break 
				 * and has the right canonical
				 * 				//return RedirectToAction("Post", "BlogPost", new { title = lpvm?.Posts?.First().PermaLink });
				 */
				lpvm.Posts = new List<PostViewModel>() { mapper.Map<PostViewModel>(entry) };
				return SinglePostView(lpvm);
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

			ValidatePostName(post);
			if (!ModelState.IsValid)
			{
				return View(post);
			}

			if (!string.IsNullOrWhiteSpace(post.NewCategory))
			{
				ModelState.AddModelError(nameof(post.NewCategory), 
					$"Please click 'Add' to add the category, \"{post.NewCategory}\" or clear the text before continuing");
				return View(post);
			}
			try
			{
				var entry = mapper.Map<NBR.Entry>(post);
				entry.Author = httpContextAccessor.HttpContext.User.Identity.Name;
				entry.Language = "en-us"; //TODO: We inject this fron http context?
				entry.Latitude = null;
				entry.Longitude = null;
				
				var sts = blogManager.UpdateEntry(entry);
				if (sts != NBR.EntrySaveState.Updated)
				{
					ModelState.AddModelError("", "Failed to edit blog post. Please check Logs for more details.");
					return View(post);
				}

				BreakSiteCache();
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
			var post = modelViewCreator.CreateBlogPostVM();

			return View(post);
		}

		[HttpPost("post/create")]
		public IActionResult CreatePost(PostViewModel post, string submit)
		{
			NBR.Entry entry = null;

			modelViewCreator.AddAllLanguages(post);
			if (submit == Constants.BlogPostAddCategoryAction)
			{
				return HandleNewCategory(post);
			}

			if (submit == Constants.UploadImageAction)
			{
				return HandleImageUpload(post);
			}

			ValidatePostName(post);
			if (!ModelState.IsValid)
			{
				return View(post);
			}
			if (!string.IsNullOrWhiteSpace(post.NewCategory))
			{
				ModelState.AddModelError(nameof(post.NewCategory),
					$"Please click 'Add' to add the category, \"{post.NewCategory}\" or clear the text before continuing");
				return View(post);
			}

			try
			{
				entry = mapper.Map<NBR.Entry>(post);

				entry.Initialize();
				entry.Author = httpContextAccessor.HttpContext.User.Identity.Name;
				entry.Language = post.Language;
				entry.Latitude = null;
				entry.Longitude = null;

				var sts = blogManager.CreateEntry(entry);
				if (sts != NBR.EntrySaveState.Added)
				{
					ModelState.AddModelError("", "Failed to create blog post. Please check Logs for more details.");
					return View(post);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Blog post create failed: {0}", ex.Message));
				ModelState.AddModelError("", "Failed to edit blog post. Please check Logs for more details.");
			}

			if (entry != null)
			{ 
				logger.LogInformation(new EventDataItem(EventCodes.EntryAdded, null, "Blog post created: {0}", entry.Title));
			}

			BreakSiteCache();

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
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Blog post delete failed: {0} {1}", postid.ToString(), ex.Message));
				RedirectToAction("Error");
			}

			BreakSiteCache();

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		[HttpGet("{posttitle}/comments")]
		[HttpGet("{year}/{month}/{day}/{posttitle}/comments")]
		[HttpGet("{posttitle}/comments/{commentid:guid}")]
		[HttpGet("post/{posttitle}/comments/{commentid:guid}")]
		public IActionResult Comment(string posttitle, string day, string month, string year)
		{
			ListPostsViewModel lpvm = null;
			NBR.Entry entry = null;
			var postguid = Guid.Empty;

			var uniquelinkdate = ValidateUniquePostDate(year, month, day);

			entry = blogManager.GetBlogPost(posttitle, uniquelinkdate);

			if (entry == null && Guid.TryParse(posttitle, out postguid))
			{
				entry = blogManager.GetBlogPostByGuid(postguid);

				var pvm = mapper.Map<PostViewModel>(entry);

				return RedirectPermanent(dasBlogSettings.GetCommentViewUrl(pvm.PermaLink));
			}

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
						Comments = blogManager.GetComments(entry.EntryId, false)
							.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
						PostId = entry.EntryId,
						PostDate = entry.CreatedUtc,
						CommentUrl = dasBlogSettings.GetCommentViewUrl(posttitle),
						ShowComments = true,
						AllowComments = entry.AllowComments
					};

					lpvm.Posts.First().Comments = lcvm;
				}
			}

			return SinglePostView(lpvm);
		}

		public IActionResult CommentError(AddCommentViewModel comment, List<string> errors)
		{
			ListPostsViewModel lpvm = null;
			NBR.Entry entry = null;
			var postguid = Guid.Parse(comment.TargetEntryId);
			entry = blogManager.GetBlogPostByGuid(postguid);
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
						Comments = blogManager.GetComments(entry.EntryId, false)
							.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
						PostId = entry.EntryId,
						PostDate = entry.CreatedUtc,
						CommentUrl = dasBlogSettings.GetCommentViewUrl(comment.TargetEntryId),
						ShowComments = true,
						AllowComments = entry.AllowComments
					};

                    if(comment != null)
                        lcvm.CurrentComment = comment;
					lpvm.Posts.First().Comments = lcvm;
                    if(errors != null && errors.Count > 0 )
                        lpvm.Posts.First().ErrorMessages = errors;
				}
			}

			return SinglePostView(lpvm);
		}



		private IActionResult Comment(string posttitle)
		{
			return Comment(posttitle, string.Empty, string.Empty, string.Empty);
		}

		[AllowAnonymous]
		[HttpPost("post/comments")]
		public IActionResult AddComment(AddCommentViewModel addcomment)
		{
            List<string> errors = new List<string>();

			if (!ModelState.IsValid)
			{
				errors.Add("[Some of your entries are invalid]");
			}

			if (!dasBlogSettings.SiteConfiguration.EnableComments)
			{
				errors.Add("Comments are disabled on the site.");
			}

			// Optional in case of Captcha. Commenting the settings in the config file 
            // Will disable this check. People will typically disable this when using captcha.
            if (!String.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.CheesySpamQ) &&
                !String.IsNullOrEmpty(dasBlogSettings.SiteConfiguration.CheesySpamA) && 
                dasBlogSettings.SiteConfiguration.CheesySpamQ.Trim().Length > 0 && 
				dasBlogSettings.SiteConfiguration.CheesySpamA.Trim().Length > 0)
			{
				if (string.Compare(addcomment.CheesyQuestionAnswered, dasBlogSettings.SiteConfiguration.CheesySpamA, 
					StringComparison.OrdinalIgnoreCase) != 0)
				{
                    errors.Add("Answer to Spam Question is invalid. Please enter a valid answer for Spam Question and try again.");
				}
			}

            if(dasBlogSettings.SiteConfiguration.EnableCaptcha)
            {
                var recaptchaTask = recaptcha.Validate(Request);
                recaptchaTask.Wait();
                var recaptchaResult = recaptchaTask.Result;
                if ((!recaptchaResult.success || recaptchaResult.score != 0) && 
                      recaptchaResult.score < dasBlogSettings.SiteConfiguration.RecaptchaMinimumScore )
                {
                    errors.Add("Unfinished Captcha. Please finish the captcha by clicking 'I'm not a robot' and try again.");
                }
            }

			if (errors.Count > 0)
			{
				return CommentError(addcomment, errors);
			}

			addcomment.Content = dasBlogSettings.FilterHtml(addcomment.Content);

			var commt = mapper.Map<NBR.Comment>(addcomment);
			commt.AuthorIPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
			commt.AuthorUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
			commt.CreatedUtc = commt.ModifiedUtc = DateTime.UtcNow;
			commt.EntryId = Guid.NewGuid().ToString();
			commt.IsPublic = !dasBlogSettings.SiteConfiguration.CommentsRequireApproval;

			var state = blogManager.AddComment(addcomment.TargetEntryId, commt);

			if (state == NBR.CommentSaveState.Failed)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Failed to save comment: {0}", commt.TargetTitle));
				errors.Add("Failed to save comment.");
			}

			if (state == NBR.CommentSaveState.SiteCommentsDisabled)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Comments are closed for this post: {0}", commt.TargetTitle));
				errors.Add("Comments are closed for this post.");
			}

			if (state == NBR.CommentSaveState.PostCommentsDisabled)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Comment are currently disabled: {0}", commt.TargetTitle));
				errors.Add("Comment are currently disabled.");
			}

			if (state == NBR.CommentSaveState.NotFound)
			{
				logger.LogError(new EventDataItem(EventCodes.CommentBlocked, null, "Invalid Post Id: {0}", commt.TargetTitle));
				errors.Add("Invalid Post Id.");
			}

			if (errors.Count > 0)
			{
				return CommentError(addcomment, errors);
			}

			logger.LogInformation(new EventDataItem(EventCodes.CommentAdded, null, "Comment created on: {0}", commt.TargetTitle));
			BreakSiteCache();
			return Comment(addcomment.TargetEntryId);
		}

		[HttpDelete("post/{postid:guid}/comments/{commentid:guid}")]
		public IActionResult DeleteComment(Guid postid, Guid commentid)
		{
			var state = blogManager.DeleteComment(postid.ToString(), commentid.ToString());

			if (state == NBR.CommentSaveState.Failed)
			{
				logger.LogError(new EventDataItem(EventCodes.Error, null, "Delete comment failed: {0}", postid.ToString()));
				return StatusCode(500);
			}

			if (state == NBR.CommentSaveState.NotFound)
			{
				return NotFound();
			}

			logger.LogInformation(new EventDataItem(EventCodes.CommentDeleted, null, "Comment deleted on: {0}", postid.ToString()));

			BreakSiteCache();

			return Ok();
		}

		[HttpPatch("post/{postid:guid}/comments/{commentid:guid}")]
		public IActionResult ApproveComment(Guid postid, Guid commentid)
		{
			var state = blogManager.ApproveComment(postid.ToString(), commentid.ToString());

			if (state == NBR.CommentSaveState.Failed)
			{
				return StatusCode(500);
			}

			if (state == NBR.CommentSaveState.NotFound)
			{
				return NotFound();
			}

			logger.LogInformation(new EventDataItem(EventCodes.CommentApproved, null, "Comment approved on: {0}", postid.ToString()));

			BreakSiteCache();

			return Ok();
		}

		[AllowAnonymous]
		[HttpGet("post/category/{category}")]
		public IActionResult GetCategory(string category)
		{
			if (string.IsNullOrWhiteSpace(category))
			{
				return RedirectToAction("Index", "Home");
			}

			var lpvm = new ListPostsViewModel();
			lpvm.Posts = categoryManager.GetEntries(category, httpContextAccessor.HttpContext.Request.Headers["Accept-Language"])
								.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();

			DefaultPage();

			ViewData[Constants.ShowPageControl] = false;
			return View(BLOG_PAGE, lpvm);
		}

		[AllowAnonymous]
		[HttpPost("post/search", Name=Constants.SearcherRouteName)]
		public IActionResult Search(string searchText)
		{
			if (string.IsNullOrWhiteSpace(searchText))
			{
				return RedirectToAction("Index", "Home");
			}

			var lpvm = new ListPostsViewModel();
			var entries = blogManager.SearchEntries(WebUtility.HtmlEncode(searchText), Request.Headers["Accept-Language"])?.Where(e => e.IsPublic)?.ToList();

			if (entries != null )
			{
				lpvm.Posts = entries.Select(entry => mapper.Map<PostViewModel>(entry)).ToList();
				ViewData[Constants.ShowPageControl] = false;

				logger.LogInformation(new EventDataItem(EventCodes.Search, null, "Search request: '{0}'", searchText));

				return View(BLOG_PAGE, lpvm);
			}

			return RedirectToAction("index", "home");
		}

		private IActionResult HandleNewCategory(PostViewModel post)
		{
			ModelState.ClearValidationState("");
			if (string.IsNullOrWhiteSpace(post.NewCategory))
			{
				ModelState.AddModelError(nameof(post.NewCategory), 
					"To add a category you must enter some text in the box next to the 'Add' button before clicking 'Add'");
				return View(post);
			}

			var newCategory = post.NewCategory?.Trim();
			var newCategoryDisplayName = newCategory;
			var newCategoryUrl = NBR.Entry.InternalCompressTitle(newCategory);
					// Category names should not include special characters #200
			if (post.AllCategories.Any(c => c.CategoryUrl == newCategoryUrl))
			{
				ModelState.AddModelError(nameof(post.NewCategory), $"The category, {post.NewCategory}, already exists");
			}
			else
			{
				post.AllCategories.Add(new CategoryViewModel { Category = newCategoryDisplayName, CategoryUrl = newCategoryUrl, Checked = true });
				post.NewCategory = "";
				ModelState.Remove(nameof(post.NewCategory));	// ensure response refreshes page with view model's value
			}

			return View(post);
		}

		private IActionResult HandleImageUpload(PostViewModel post)
		{
			ModelState.ClearValidationState("");
			var fileName = post.Image?.FileName;
			if (string.IsNullOrEmpty(fileName))
			{
				ModelState.AddModelError(nameof(post.Image), 
						$"You must select a file before clicking \"{Constants.UploadImageAction}\" to upload it");
				return View(post);
			}

			string fullimageurl = null;
			try
			{
				using (var s = post.Image.OpenReadStream())
				{
					fullimageurl = binaryManager.SaveFile(s, Path.GetFileName(fileName));
				}
			}
			catch (Exception e)
			{
				ModelState.AddModelError(nameof(post.Image), $"An error occurred while uploading image ({e.Message})");
				return View(post);
			}

			if (string.IsNullOrEmpty(fullimageurl))
			{
				ModelState.AddModelError(nameof(post.Image), "Failed to upload file - reason unknown");
				return View(post);
			}

			post.Content += string.Format("<p><img border=\"0\" src=\"{0}\"></p>", fullimageurl);
			ModelState.Remove(nameof(post.Content)); // ensure that model change is included in response
			return View(post);
		}

		private void ValidatePostName(PostViewModel post)
		{
			var dt = ValidatePostDate(post);
			var entry = blogManager.GetBlogPost(post.Title.Replace(" ", string.Empty), dt);

			if (entry != null && string.Compare(entry.EntryId, post.EntryId, true) > 0 )
			{
				ModelState.AddModelError(string.Empty, "A post with this title already exists. Titles must be unique");
			}
		}

		private DateTime? ValidateUniquePostDate(string year, string month, string day)
		{
			DateTime? LinkUniqueDate = null;

			if (dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique)
			{
				int.TryParse(string.Format("{0}{1}{2}", year, month, day), out var dayYear);

				if (dayYear > 0)
				{
					LinkUniqueDate = DateTime.ParseExact(dayYear.ToString(), "yyyyMMdd", null, DateTimeStyles.AdjustToUniversal);
				}
			}

			return LinkUniqueDate;
		}

		private DateTime? ValidatePostDate(PostViewModel postView)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique)
			{
				return null;
			}

			return postView?.CreatedDateTime;
		}

		private void BreakSiteCache()
		{
			memoryCache.Remove(CACHEKEY_RSS);
			memoryCache.Remove(CACHEKEY_FRONTPAGE);
		}

	}
}
