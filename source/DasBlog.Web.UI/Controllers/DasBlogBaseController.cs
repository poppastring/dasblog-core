using DasBlog.Core;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Settings
{
	public abstract class DasBlogBaseController : DasBlogController
	{
		private readonly IDasBlogSettings dasBlogSettings;

		protected DasBlogBaseController(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;
		}

		//protected ViewResult ThemedView(string view, ListPostsViewModel listpostsviewmodel)
		//{
		//	return View($"/Themes/{_dasBlogSettings.SiteConfiguration.Theme}/{view}.cshtml", listpostsviewmodel);
		//}

		protected void SinglePost(PostViewModel post)
		{
			if (post != null)
			{
				ViewData["Title"] = post.Title;
				ViewData["Description"] = post.Description;
				ViewData["Keywords"] = post.Categories;
				ViewData["Canonical"] = post.PermaLink;
				ViewData["Author"] = post.Author;
			}
			else
			{
				DefaultPage();
			}
		}

		protected void DefaultPage()
		{
			ViewData["Title"] = dasBlogSettings.SiteConfiguration.Title;
			ViewData["Description"] = dasBlogSettings.SiteConfiguration.Description;
			ViewData["Keywords"] = dasBlogSettings.MetaTags.MetaKeywords;
			ViewData["Canonical"] = dasBlogSettings.SiteConfiguration.Root;
			ViewData["Author"] = dasBlogSettings.SiteConfiguration.Copyright;
		}
	}
}
