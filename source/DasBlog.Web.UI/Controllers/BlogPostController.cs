using AutoMapper;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using DasBlog.Web.Settings;
using DasBlog.Core;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	[Route("post")]
	public class BlogPostController : DasBlogBaseController
	{
		private IBlogManager _blogManager;
		private IHttpContextAccessor _httpContextAccessor;
		private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public BlogPostController(IBlogManager blogManager, IHttpContextAccessor httpContextAccessor, 
									IDasBlogSettings settings, IMapper mapper) : base(settings)
		{
			_blogManager = blogManager;
			_httpContextAccessor = httpContextAccessor;
			_dasBlogSettings = settings;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("edit/{postid:guid}")]
		public IActionResult EditPost(Guid postid)
		{
			PostViewModel pvm = new PostViewModel();

			if (!string.IsNullOrEmpty(postid.ToString()))
			{
				var entry = _blogManager.GetEntryForEdit(postid.ToString());
				if (entry != null)
				{
					pvm = _mapper.Map<PostViewModel>(entry);
					List <CategoryViewModel> allcategories = _mapper.Map<List<CategoryViewModel>>(_blogManager.GetCategories());

					foreach(var cat in allcategories)
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

		[HttpPost]
		[Route("edit")]
		public IActionResult EditPost(PostViewModel post)
		{
			if (!ModelState.IsValid)
			{
				return View(post);
			}

			try
			{
				Entry entry = _mapper.Map<Entry>(post);
				
				entry.Author = _httpContextAccessor.HttpContext.User.Identity.Name;
				entry.Language = "en-us"; //TODO: We inject this fron http context?
				entry.Latitude = null;
				entry.Longitude = null;

				EntrySaveState sts = _blogManager.UpdateEntry(entry);
				if (sts != EntrySaveState.Updated)
				{
					ModelState.AddModelError("", "Failed to edit blog post");
					return View(post);
				}
			}
			catch(Exception e)
			{
				RedirectToAction("Error");
			}

			return View(post);
		}

		[HttpGet]
		[Route("create")]
		public IActionResult CreatePost()
		{
			PostViewModel post = new PostViewModel();
			post.CreatedDateTime = DateTime.UtcNow;  //TODO: Set to the timezone configured???
			post.AllCategories = _mapper.Map<List<CategoryViewModel>>(_blogManager.GetCategories());

			return View(post);
		}

		[HttpPost]
		[Route("create")]
		public IActionResult CreatePost(PostViewModel post)
		{
			if (!ModelState.IsValid)
			{
				return View(post);
			}

			try
			{
				Entry entry = _mapper.Map<Entry>(post);

				entry.Initialize();
				entry.Author = _httpContextAccessor.HttpContext.User.Identity.Name;
				entry.Language = "en-us"; //TODO: We inject this fron http context?
				entry.Latitude = null;
				entry.Longitude = null;

				EntrySaveState sts = _blogManager.CreateEntry(entry);
				if(sts != EntrySaveState.Added)
				{
					ModelState.AddModelError("", "Failed to create blog post");
					return View(post);
				}
			}
			catch (Exception e)
			{
				RedirectToAction("Error");
			}

			return View("Views/BlogPost/EditPost.cshtml", post);
		}

		[HttpGet]
		[Route("delete/{postid:guid}")]
		public IActionResult DeletePost(Guid postid)
		{
			try
			{
				_blogManager.DeleteEntry(postid.ToString());
			}
			catch(Exception ex)
			{
				RedirectToAction("Error");
			}

			return RedirectToAction("Index", "Home");
		}

		[AllowAnonymous]
		[HttpGet("{postid:guid}/comment")]
		public IActionResult Comment(Guid postid)
		{
			// TODO are comments enabled?

			Entry entry = _blogManager.GetBlogPost(postid.ToString());

			ListPostsViewModel lpvm = new ListPostsViewModel();
			lpvm.Posts = new List<PostViewModel> { _mapper.Map<PostViewModel>(entry) };

			ListCommentsViewModel lcvm = new ListCommentsViewModel
			{
				Comments = _blogManager.GetComments(postid.ToString(), false)
					.Select(comment => _mapper.Map<CommentViewModel>(comment)).ToList(),
				PostId = postid.ToString()
			};

			lpvm.Posts.First().Comments = lcvm;

			SinglePost(lpvm.Posts.First());

			return ThemedView("Page", lpvm);
		}

		[AllowAnonymous]
		[HttpPost]
		[Route("comment")]
		public IActionResult AddComment([FromForm]AddCommentViewModel comment)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}

			Comment commt = _mapper.Map<Comment>(comment);
			CommentSaveState state = _blogManager.AddComment(comment.TargetEntryId.ToString(), commt);

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

		[HttpDelete]
		[Route("{postid:guid}/comment/{commentid:guid}")]
		public IActionResult DeleteComment(Guid postid, Guid commentid)
		{
			CommentSaveState state = _blogManager.DeleteComment(postid.ToString(), commentid.ToString());

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

		[HttpPatch]
		[Route("{postid:guid}/comment/{commentid:guid}")]
		public IActionResult ApproveComment(Guid postid, Guid commentid)
		{
			CommentSaveState state = _blogManager.ApproveComment(postid.ToString(), commentid.ToString());

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
	}
}
