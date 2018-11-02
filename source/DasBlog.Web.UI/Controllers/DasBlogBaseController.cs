using System.Linq;
using DasBlog.Core;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Settings
{
	public abstract class DasBlogBaseController : DasBlogController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		protected const string BLOG_PAGE = "_BlogPage";

		protected DasBlogBaseController(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;
		}

		protected void SinglePost(PostViewModel post)
		{
			if (post != null)
			{
				ViewData["PageTitle"] = post.Title;
				ViewData["Description"] = post.Title;
				ViewData["Keywords"] = string.Join(",", post.Categories.Select(x => x.Category).ToArray());
				ViewData["Canonical"] = post.PermaLink;
				ViewData["Author"] = post.Author;
			}
			else
			{
				DefaultPage();
			}
		}

		protected void DefaultPage(string pageTitle = "")
		{
			if (pageTitle.Length > 0)
			{
				ViewData["PageTitle"] = string.Format("{0} - {1}", pageTitle, dasBlogSettings.SiteConfiguration.Title);
				ViewData["Description"] = string.Format("{0} - {1}", pageTitle, dasBlogSettings.SiteConfiguration.Description);
				ViewData["Keywords"] = string.Empty;
				ViewData["Canonical"] = string.Empty;
				ViewData["Author"] = dasBlogSettings.SiteConfiguration.Copyright;
			}
			else
			{
				ViewData["PageTitle"] = dasBlogSettings.SiteConfiguration.Title;
				ViewData["Description"] = dasBlogSettings.SiteConfiguration.Description;
				ViewData["Keywords"] = dasBlogSettings.MetaTags.MetaKeywords;
				ViewData["Canonical"] = dasBlogSettings.SiteConfiguration.Root;
				ViewData["Author"] = dasBlogSettings.SiteConfiguration.Copyright;
			}
		}
	}
}
