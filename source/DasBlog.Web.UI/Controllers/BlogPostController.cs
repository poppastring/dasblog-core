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
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using newtelligence.DasBlog.Runtime;
using static DasBlog.Core.Common.Utils;

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

		public BlogPostController(IBlogManager blogManager, IHttpContextAccessor httpContextAccessor,
		  IDasBlogSettings settings, IMapper mapper, ICategoryManager categoryManager
		  ,IFileSystemBinaryManager binaryManager) : base(settings)
		{
			this.blogManager = blogManager;
			this.categoryManager = categoryManager;
			this.httpContextAccessor = httpContextAccessor;
			dasBlogSettings = settings;
			this.mapper = mapper;
			this.binaryManager = binaryManager;
		}

		[AllowAnonymous]
		public IActionResult Post(string posttitle)
		{
			ListPostsViewModel lpvm = new ListPostsViewModel();

			if (!string.IsNullOrEmpty(posttitle))
			{
				var entry = blogManager.GetBlogPost(posttitle.Replace(dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement, string.Empty));
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
					pvm.Languages = GetAlllanguages();
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
			post.Languages = GetAlllanguages();
			if (submit == Constants.BlogPostAddCategoryAction)
			{
				return HandleNewCategory(post);
			}
			if (submit == Constants.UploadImageAction)
			{
				return HandleImageUpload(post);
			}
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
					ModelState.AddModelError("", "Failed to edit blog post");
					return View(post);
				}
			}
			catch (Exception e)
			{
				RedirectToAction("Error");
			}

			return View(post);
		}

		[HttpGet("post/create")]
		public IActionResult CreatePost()
		{
			PostViewModel post = new PostViewModel();
			post.CreatedDateTime = DateTime.UtcNow;  //TODO: Set to the timezone configured???
			post.AllCategories = mapper.Map<List<CategoryViewModel>>(blogManager.GetCategories());
			post.Languages = GetAlllanguages();

			return View(post);
		}

		[HttpPost("post/create")]
		public IActionResult CreatePost(PostViewModel post, string submit)
		{
			post.Languages = GetAlllanguages();
			if (submit == Constants.BlogPostAddCategoryAction)
			{
				return HandleNewCategory(post);
			}

			if (submit == Constants.UploadImageAction)
			{
				return HandleImageUpload(post);
			}
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
					ModelState.AddModelError("", "Failed to create blog post");
					return View(post);
				}
			}
			catch (Exception e)
			{
				RedirectToAction("Error");
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
				RedirectToAction("Error");
			}

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		[HttpGet("post/{postid:guid}/comment")]
		public IActionResult Comment(Guid postid)
		{
			// TODO are comments enabled?

			Entry entry = blogManager.GetBlogPost(postid.ToString());

			ListPostsViewModel lpvm = new ListPostsViewModel();
			lpvm.Posts = new List<PostViewModel> { mapper.Map<PostViewModel>(entry) };

			ListCommentsViewModel lcvm = new ListCommentsViewModel
			{
				Comments = blogManager.GetComments(postid.ToString(), false)
					.Select(comment => mapper.Map<CommentViewModel>(comment)).ToList(),
				PostId = postid.ToString()
			};

			lpvm.Posts.First().Comments = lcvm;

			SinglePost(lpvm.Posts.First());

			return View("page", lpvm);
		}

		[AllowAnonymous]
		[HttpPost("post/comment")]
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

			Comment commt = mapper.Map<Comment>(addcomment);
			commt.AuthorIPAddress = HttpContext.Connection.RemoteIpAddress.ToString();
			commt.AuthorUserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
			commt.CreatedUtc = commt.ModifiedUtc = DateTime.UtcNow;
			commt.EntryId = Guid.NewGuid().ToString();
			commt.IsPublic = !dasBlogSettings.SiteConfiguration.CommentsRequireApproval;

			CommentSaveState state = blogManager.AddComment(addcomment.TargetEntryId, commt);

			if (state == CommentSaveState.Failed)
			{
				ModelState.AddModelError("", "Comment failed");
				return StatusCode(500);
			}

			if (state == CommentSaveState.NotFound)
			{
				ModelState.AddModelError("", "Invalid comment attempt");
				return NotFound();
			}

			return Comment(new Guid(addcomment.TargetEntryId));
		}

		[HttpDelete("post/{postid:guid}/comment/{commentid:guid}")]
		public IActionResult DeleteComment(Guid postid, Guid commentid)
		{
			CommentSaveState state = blogManager.DeleteComment(postid.ToString(), commentid.ToString());

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

		[HttpPatch("post/{postid:guid}/comment/{commentid:guid}")]
		public IActionResult ApproveComment(Guid postid, Guid commentid)
		{
			CommentSaveState state = blogManager.ApproveComment(postid.ToString(), commentid.ToString());

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
		[HttpPost]
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
			var newCategoryDisplayName = newCategory;
			var newCategoryUrl = EncodeCategoryUrl(newCategory, dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement );
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
					,$"You must select a file before clicking \"{Constants.UploadImageAction}\" to upload it");
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
				  ,"Failed to upload file - reason unknown");
				return View(post);
			}
			string linkText = String.Format("<p><img border=\"0\" src=\"{0}\"></p>",
				relativePath);
			post.Content += linkText;
			ModelState.Remove(nameof(post.Content));	// ensure that model change is included in response
			return View(post);
		}
		private IEnumerable<SelectListItem> GetAlllanguages()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			// setup temp store for listitem items, for sorting
			List<SelectListItem> cultureList = new List<SelectListItem>(cultures.Length);

			foreach (CultureInfo ci in cultures)
			{
				string langName = (ci.NativeName != ci.EnglishName) ? ci.NativeName + " / " + ci.EnglishName : ci.NativeName;

				if (langName.Length > 55)
				{
					langName = langName.Substring(0, 55) + "...";
				}

				if (string.IsNullOrEmpty(ci.Name))
				{
					langName = string.Empty;		// invariant language (invariant country)
				}

				cultureList.Add(new SelectListItem{ Value = ci.Name, Text = langName});
			}

			// setup the sort culture
			//string rssCulture = requestPage.SiteConfig.RssLanguage;

			CultureInfo sortCulture;


			try
			{
//				sortCulture = (rssCulture != null && rssCulture.Length > 0 ? new CultureInfo(rssCulture) : CultureInfo.CurrentCulture);
				sortCulture = CultureInfo.CurrentCulture;
			}
			catch (ArgumentException)
			{
				// default to the culture of the server
				sortCulture = CultureInfo.CurrentCulture;
			}

			// sort the list
			cultureList.Sort(delegate(SelectListItem x, SelectListItem y)
			{
				// actual comparison
				return String.Compare(x.Text, y.Text, true, sortCulture);
			});
			// add to the languages listbox

			SelectListItem[] cultureListItems = cultureList.ToArray();

			return cultureListItems;
		}
	}
}
