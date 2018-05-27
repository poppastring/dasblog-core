using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Settings
{
    public class DasBlogBaseController : Controller
    {
		private readonly IDasBlogSettings _dasBlogSettings;

		public DasBlogBaseController(IDasBlogSettings settings)
		{
			_dasBlogSettings = settings;
		}

		protected ViewResult ThemedView(string view, ListPostsViewModel listpostsviewmodel)
		{
			return View(string.Format("/Themes/{0}/{1}.cshtml",
						_dasBlogSettings.SiteConfiguration.Theme, view), listpostsviewmodel);
		}

		protected void SinglePost(PostViewModel post)
		{
			ViewData["Title"] = post.Title;
			ViewData["Description"] = post.Description;
			ViewData["Keywords"] = post.Categories;
			ViewData["Canonical"] = post.PermaLink;
			ViewData["Author"] = post.Author;
		}

		protected void DefaultPage()
		{
			ViewData["Title"] = _dasBlogSettings.SiteConfiguration.Title;
			ViewData["Description"] = _dasBlogSettings.SiteConfiguration.Description;
			ViewData["Keywords"] = _dasBlogSettings.MetaTags.MetaKeywords;
			ViewData["Canonical"] = _dasBlogSettings.SiteConfiguration.Root;
			ViewData["Author"] = _dasBlogSettings.SiteConfiguration.Copyright;
		}
	}
}
