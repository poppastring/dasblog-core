using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Web.Models;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using AutoMapper;

namespace DasBlog.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IBlogManager _blogManager;
        private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public HomeController(IBlogManager blogManager, IDasBlogSettings settings, IMapper mapper)
        {
            _blogManager = blogManager;
            _dasBlogSettings = settings;
			_mapper = mapper;
		}

        public IActionResult Index()
        {
			ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts = _blogManager.GetFrontPagePosts()
                            .Select(entry => _mapper.Map<PostViewModel>(entry)).ToList();
            DefaultPage();

            return ThemedView("Page", lpvm);
        }

		//TODO: Maybe a helper for all?
		private ViewResult ThemedView(string v, ListPostsViewModel lpvm)
		{
			return View(string.Format("/Themes/{0}/{1}.cshtml", 
						_dasBlogSettings.SiteConfiguration.Theme, v), lpvm);
		}

		[HttpGet]
		public IActionResult Post(string posttitle)
        {
            ListPostsViewModel lpvm = new ListPostsViewModel();

            if (!string.IsNullOrEmpty(posttitle))
            {
                var entry = _blogManager.GetBlogPost(posttitle.Replace(_dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement,string.Empty));
                if (entry != null)
                {
					lpvm.Posts = new List<PostViewModel>() {_mapper.Map<PostViewModel>(entry) };

                    SinglePost(lpvm.Posts.First());

					return ThemedView("Page", lpvm);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return Index();
            }
        }

		[HttpGet("comment/{postid:guid}")]
        public IActionResult Comment(Guid postid)
        {
            // ~/CommentView.aspx?title=GeneralPatternsusedtoDetectaLeak

            // Get post by GUID bbecae4b-e3a3-47a2-b6a6-b4cc405f8663
            Entry entry = _blogManager.GetBlogPost(postid.ToString());

            ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts = new List<PostViewModel> { _mapper.Map<PostViewModel>(entry) };

            SinglePost(lpvm.Posts.First());

			return ThemedView("Page", lpvm);
        }

        [Route("page")]
        public IActionResult Page()
        {
            return Index();
        }

        [Route("page/{index:int}")]
        public IActionResult Page(int index)
        {
            if (index == 0)
            {
                return Index();
            }

            ViewData["Message"] = string.Format("Page...{0}", index);

            ListPostsViewModel lpvm = new ListPostsViewModel();
            lpvm.Posts = _blogManager.GetEntriesForPage(index)
                                .Select(entry => _mapper.Map<PostViewModel>(entry)).ToList();

            DefaultPage();

			return ThemedView("Page", lpvm);
        }

        [HttpGet("blogger")]
        public ActionResult Blogger()
        {
            // https://www.poppastring.com/blog/blogger.aspx
            // Implementation of Blogger XML-RPC Api
            // blogger
            // metaWebLog
            // mt

            return NoContent();
        }

        [Route("blogger")]
        [HttpPost]
        [Produces("text/xml")]
        public IActionResult Blogger([FromBody] string xmlrpcpost)
        {
            return this.Content(_blogManager.XmlRpcInvoke(this.HttpContext.Request.Body));
        }

        public IActionResult About()
        {
            DefaultPage();

            ViewData["Message"] = "Your application description page.";

            return NoContent();
        }

        public IActionResult Contact()
        {
            DefaultPage();

            ViewData["Message"] = "Your contact page.";

            return NoContent();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private void SinglePost(PostViewModel post)
        {
            ViewData["Title"] = post.Title;
            ViewData["Description"] = post.Description;
            ViewData["Keywords"] = post.Categories;
            ViewData["Canonical"] = post.PermaLink;
            ViewData["Author"] = post.Author;
        }

        private void DefaultPage()
        {
            ViewData["Title"] = _dasBlogSettings.SiteConfiguration.Title;
            ViewData["Description"] = _dasBlogSettings.SiteConfiguration.Description;
            ViewData["Keywords"] = _dasBlogSettings.MetaTags.MetaKeywords;
            ViewData["Canonical"] = _dasBlogSettings.SiteConfiguration.Root;
            ViewData["Author"] = _dasBlogSettings.SiteConfiguration.Copyright;
        }
    }
}

