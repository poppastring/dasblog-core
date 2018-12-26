using System;
using System.Linq;
using DasBlog.Core;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Core.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.Settings
{
	public abstract class DasBlogBaseController : DasBlogController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		protected const string BLOG_PAGE = "_BlogPage";
		protected const string BLOG_PAGESUMMARY = "_BlogPageSummary";

		protected DasBlogBaseController(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;
		}

		protected ViewResult SinglePostView(ListPostsViewModel listPostsViewModel)
		{
			SinglePost(listPostsViewModel?.Posts?.First());

			return View(BLOG_PAGE, listPostsViewModel);
		}

		protected ViewResult AggregatePostView(ListPostsViewModel listPostsViewModel)
		{
			DefaultPage();

			if (dasBlogSettings.SiteConfiguration.ShowItemDescriptionInAggregatedViews)
			{
				listPostsViewModel = EditContentDescription(listPostsViewModel);
			}

			if (dasBlogSettings.SiteConfiguration.ShowItemSummaryInAggregatedViews)
			{
				return View(BLOG_PAGESUMMARY, listPostsViewModel);
			}

			return View(BLOG_PAGE, listPostsViewModel);
		}

		protected void SinglePost(PostViewModel post)
		{
			if (post != null)
			{
				ViewData["PageTitle"] = post.Title;
				ViewData["Description"] = post.Description.StripHTMLFromText().CutLongString(80); 
				ViewData["Keywords"] = string.Join(",", post.Categories.Select(x => x.Category).ToArray());
				ViewData["Canonical"] = post.PermaLink;
				ViewData["Author"] = post.Author;
				ViewData["PageImageUrl"] = (post.ImageUrl?.Length > 0) ? post.ImageUrl : dasBlogSettings.MetaTags.TwitterImage;
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
				ViewData["PageImageUrl"] = dasBlogSettings.MetaTags.TwitterImage;
			}
			else
			{
				ViewData["PageTitle"] = dasBlogSettings.SiteConfiguration.Title;
				ViewData["Description"] = dasBlogSettings.SiteConfiguration.Description;
				ViewData["Keywords"] = dasBlogSettings.MetaTags.MetaKeywords;
				ViewData["Canonical"] = dasBlogSettings.SiteConfiguration.Root;
				ViewData["Author"] = dasBlogSettings.SiteConfiguration.Copyright;
				ViewData["PageImageUrl"] = dasBlogSettings.MetaTags.TwitterImage;
			}
		}

		private ListPostsViewModel EditContentDescription(ListPostsViewModel listPostsViewModel)
		{
			if (dasBlogSettings.SiteConfiguration.ShowItemDescriptionInAggregatedViews)
			{
				if (listPostsViewModel != null && listPostsViewModel.Posts != null)
				{
					foreach (var post in listPostsViewModel.Posts)
					{
						post.Content = post.Description;
					}
				}
			}

			return listPostsViewModel;
		}
	}
}
