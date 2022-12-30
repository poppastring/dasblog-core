
using DasBlog.Core.Common;
using DasBlog.Core.Extensions;
using DasBlog.Services;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace DasBlog.Web.Settings
{
	public abstract class DasBlogBaseController : DasBlogController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		protected const string BLOG_PAGE = "_BlogPage";
		protected const string BLOG_PAGESUMMARY = "_BlogPageSummary";
		protected const string BLOG_EMAIL_COMMENT_SUBJECT = "Weblog comment by {0} from {1} on {2}";
		protected const string BLOG_EMAIL_COMMENT_TEMPLATE_BODY = "@Model.Comment \r\n\r\n Comment Page @Model.CommentUrl \r\n Login and approve/delete the comment.";
		protected readonly JsonSerializerOptions jsonSerializerOptions;

		protected DasBlogBaseController(IDasBlogSettings settings)
		{
			dasBlogSettings = settings;
			jsonSerializerOptions = new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
		}

		protected ViewResult SinglePostView(ListPostsViewModel listPostsViewModel)
		{
	    	SinglePost(listPostsViewModel?.Posts?.First());
			ViewData[Constants.ShowPageControl] = false;
			return View(BLOG_PAGE, listPostsViewModel);
		}

		protected ViewResult AggregatePostView(ListPostsViewModel listPostsViewModel)
		{
			DefaultPage();

			if (dasBlogSettings.SiteConfiguration.ShowItemSummaryInAggregatedViews)
			{
				return View(BLOG_PAGESUMMARY, listPostsViewModel);
			}

			return View(BLOG_PAGE, listPostsViewModel);
		}

		protected void SinglePost(PostViewModel post)
		{
			DefaultPage();
			if (post != null)
			{
				ViewData["PageTitle"] = post.Title;
				ViewData["CreatedTime"] = post.CreatedDateTime.ToString("R");
				ViewData["ModifiedTime"] = post.ModifiedDateTime.ToString("R");
				if (string.IsNullOrEmpty(post.Description))
				{
					ViewData["Description"] = post.Content.StripHTMLFromText().CutLongString(80);
				}
				else
				{
					ViewData["Description"] = post.Description.StripHTMLFromText().CutLongString(80);
				}
				ViewData["PermaLink"] = dasBlogSettings.RelativeToRoot(post.PermaLink);
				ViewData["Keywords"] = string.Join(",", post.Categories.Select(x => x.Category).ToArray());
				ViewData["Canonical"] = dasBlogSettings.RelativeToRoot(post.PermaLink);
				ViewData["Author"] = dasBlogSettings.GetUserByEmail(post.Author)?.DisplayName; ;
				ViewData["PageImageUrl"] = (post.ImageUrl?.Length > 0) ? dasBlogSettings.RelativeToRoot(post.ImageUrl) : dasBlogSettings.MetaTags.TwitterImage;
				ViewData["PageVideoUrl"] = (post.VideoUrl?.Length > 0) ? dasBlogSettings.RelativeToRoot(post.VideoUrl) : string.Empty;
                ShowErrors(post);
			}
		}

        private void ShowErrors(PostViewModel post)
        {
            if(post != null && post.Comments != null && post.Comments.CurrentComment != null && post.ErrorMessages != null && post.ErrorMessages.Count > 0)
            {
                foreach(string ErrorMessage in post.ErrorMessages)
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
            }
        }

        protected void DefaultPage(string pageTitle = "")
		{
			ViewData["PermaLink"] = dasBlogSettings.GetBaseUrl();
			ViewData["TwitterCreator"] = dasBlogSettings.MetaTags.TwitterCreator;
			ViewData["TwitterImage"] = dasBlogSettings.MetaTags.TwitterImage;
			ViewData["TwitterSite"] = dasBlogSettings.MetaTags.TwitterSite;
			ViewData["TwitterCard"] = dasBlogSettings.MetaTags.TwitterCard;
			if (pageTitle.Length > 0)
			{
				ViewData["PageTitle"] = string.Format("{0} - {1}", pageTitle, dasBlogSettings.SiteConfiguration.Title);
				ViewData["SiteTitle"] = dasBlogSettings.SiteConfiguration.Title;
				ViewData["Description"] = string.Format("{0} - {1}", pageTitle, dasBlogSettings.MetaTags.MetaDescription);
				ViewData["Keywords"] = string.Empty;
				ViewData["Canonical"] = string.Empty;
				ViewData["Author"] = dasBlogSettings.SiteConfiguration.Copyright;
				ViewData["PageImageUrl"] = dasBlogSettings.MetaTags.TwitterImage;
				ViewData["PageVideoUrl"] = string.Empty;
			}
			else
			{
				ViewData["PageTitle"] = dasBlogSettings.SiteConfiguration.Title;
				ViewData["SiteTitle"] = dasBlogSettings.SiteConfiguration.Title;
				ViewData["Description"] = dasBlogSettings.MetaTags.MetaDescription;
				ViewData["Keywords"] = dasBlogSettings.MetaTags.MetaKeywords;
				ViewData["Canonical"] = dasBlogSettings.SiteConfiguration.Root;
				ViewData["Author"] = dasBlogSettings.SiteConfiguration.Copyright;
				ViewData["PageImageUrl"] = dasBlogSettings.MetaTags.TwitterImage;
				ViewData["PageVideoUrl"] = string.Empty;
			}
		}
	}
}
