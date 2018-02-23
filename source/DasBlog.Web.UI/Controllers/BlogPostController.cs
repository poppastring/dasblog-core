using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web.Core;
using DasBlog.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using DasBlog.Web.UI.Models.BlogViewModels;

namespace DasBlog.Web.UI.Controllers
{
	//[Authorize] this needs to be enabled...
	[Route("post")]
	public class BlogPostController : Controller
	{
		private IBlogRepository _blogRepository;
		private IHttpContextAccessor _httpContextAccessor;

		public BlogPostController(IBlogRepository blogRepository, IHttpContextAccessor httpContextAccessor)
		{
			_blogRepository = blogRepository;
			_httpContextAccessor = httpContextAccessor;
		}

		[HttpGet]
		[Route("edit/{postid:guid}")]
		public IActionResult EditPost(Guid postid)
		{
			PostViewModel pvm = new PostViewModel();

			if (!string.IsNullOrEmpty(postid.ToString()))
			{
				var entry = _blogRepository.GetBlogPost(postid.ToString());
				if (entry != null)
				{
					pvm = new PostViewModel {
						Author = entry.Author,
						Content = entry.Content,
						Categories = entry.Categories,
						Description = entry.Description,
						EntryId = entry.EntryId,
						AllowComments = entry.AllowComments,
						IsPublic = entry.IsPublic,
						PermaLink = entry.Link,
						Title = entry.Title,
						CreatedDateTime = entry.CreatedLocalTime
						};

					return View(pvm);
				}
				else
				{
					return NotFound();
				}
			}

			return NotFound();
		}

		[HttpPost]
		[Route("edit")]
		public IActionResult EditPost(PostViewModel post)
		{
			try
			{
				Entry entry = new Entry();
				entry.Initialize();

				entry.Title = post.Title;
				entry.Content = post.Content;
				entry.Description = post.Description;
				entry.Categories = post.Categories;
				entry.CreatedLocalTime = post.CreatedDateTime;
				entry.ModifiedLocalTime = DateTime.UtcNow;
				entry.Author = "admin"; // Should we inject this?
				entry.Latitude = null;
				entry.Longitude = null;
				entry.Language = "en-us";
				entry.IsPublic = post.IsPublic;
				entry.Syndicated = post.Syndicate;
				entry.AllowComments = post.AllowComments;

				_blogRepository.UpdateEntry(entry);
			}
			catch
			{
				RedirectToAction("Some error location");
			}

			return RedirectToAction("Home/Index");
		}

		[HttpGet]
		[Route("create")]
		public IActionResult CreatePost()
		{
			return NotFound();
		}

		[HttpPost]
		[Route("create")]
		public IActionResult CreatePost(PostViewModel post)
		{
			try
			{
				// retrieve entry based on id
				// compare to postviewmodel????

				_blogRepository.SaveEntry(new Entry());
			}
			catch
			{
				RedirectToAction("Some error location");
			}

			return RedirectToAction("Home/Index");
		}

		[HttpGet]
		[Route("delete/{postid:guid}")]
		public IActionResult DeletePost(Guid postid)
		{
						
			return NotFound();
		}

		[HttpPost]
		[Route("delete/{postid:guid}")]
		public IActionResult DeletePost(Guid postid, PostViewModel post)
		{
			try
			{
				_blogRepository.DeleteEntry(postid.ToString());
			}
			catch
			{
				RedirectToAction("Some error location");
			}

			return RedirectToAction("Home/Index");
		}
	}
}
