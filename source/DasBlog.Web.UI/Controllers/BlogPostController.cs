using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web;
using DasBlog.Managers.Interfaces;
using Microsoft.AspNetCore.Http;
using DasBlog.Web.Models.BlogViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace DasBlog.Web.Controllers
{
	[Authorize]
	[Route("post")]
	public class BlogPostController : Controller
	{
		private IBlogRepository _blogRepository;
		private IHttpContextAccessor _httpContextAccessor;
		private readonly IMapper _mapper;

		public BlogPostController(IBlogRepository blogRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
		{
			_blogRepository = blogRepository;
			_httpContextAccessor = httpContextAccessor;
			_mapper = mapper;
		}

		[HttpGet]
		[Route("edit/{postid:guid}")]
		public IActionResult EditPost(Guid postid)
		{
			PostViewModel pvm = new PostViewModel();

			if (!string.IsNullOrEmpty(postid.ToString()))
			{
				var entry = _blogRepository.GetEntryForEdit(postid.ToString());
				if (entry != null)
				{
					pvm = _mapper.Map<PostViewModel>(entry);
					List <CategoryViewModel> allcategories = _mapper.Map<List<CategoryViewModel>>(_blogRepository.GetCategories());

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
				
				entry.Author = "admin"; //TODO: Need to integrate with context security 
				entry.Language = "en-us"; //TODO: We inject this fron http context?
				entry.Latitude = null;
				entry.Longitude = null;

				EntrySaveState sts = _blogRepository.UpdateEntry(entry);
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
			post.AllCategories = _mapper.Map<List<CategoryViewModel>>(_blogRepository.GetCategories());

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
				entry.Author = "admin"; //TODO: Need to integrate with context security 
				entry.Language = "en-us"; //TODO: We inject this fron http context?
				entry.Latitude = null;
				entry.Longitude = null;

				EntrySaveState sts = _blogRepository.CreateEntry(entry);
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
				_blogRepository.DeleteEntry(postid.ToString());
			}
			catch(Exception ex)
			{
				RedirectToAction("Error");
			}

			return RedirectToAction("Index", "Home");
		}
	}
}
