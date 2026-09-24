using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Web.ViewComponents
{
	public class RelatedPostsViewComponent : ViewComponent
	{
		private const int DefaultCount = 5;

		private readonly IBlogManager blogManager;
		private readonly IUrlResolver urlResolver;

		public RelatedPostsViewComponent(IBlogManager blogManager, IUrlResolver urlResolver)
		{
			this.blogManager = blogManager;
			this.urlResolver = urlResolver;
		}

		public IViewComponentResult Invoke(PostViewModel post, int count = DefaultCount)
		{
			if (post == null || count <= 0)
			{
				return Content(string.Empty);
			}

			var currentCategories = post.Categories
				.Where(category => !string.IsNullOrWhiteSpace(category.Category))
				.Select(category => category.Category.Trim())
				.ToHashSet(StringComparer.OrdinalIgnoreCase);
			var now = DateTime.UtcNow;

			var relatedPosts = blogManager.GetAllEntries()
				.Where(entry => entry.IsPublic
					&& entry.CreatedUtc <= now
					&& !string.Equals(entry.EntryId, post.EntryId, StringComparison.OrdinalIgnoreCase))
				.Select(entry => new
				{
					Entry = entry,
					SharedCategoryCount = entry.GetSplitCategories()
						.Select(category => category.Trim())
						.Count(currentCategories.Contains)
				})
				.OrderByDescending(candidate => candidate.SharedCategoryCount)
				.ThenByDescending(candidate => candidate.Entry.CreatedUtc)
				.ThenBy(candidate => candidate.Entry.EntryId, StringComparer.OrdinalIgnoreCase)
				.ThenBy(candidate => candidate.Entry.Title, StringComparer.OrdinalIgnoreCase)
				.Take(count)
				.Select(candidate => new RelatedPostViewModel
				{
					Title = candidate.Entry.Title,
					Url = urlResolver.RelativeToRoot(urlResolver.GeneratePostUrl(candidate.Entry)),
					CreatedUtc = candidate.Entry.CreatedUtc
				})
				.ToList();

			if (relatedPosts.Count == 0)
			{
				return Content(string.Empty);
			}

			return View(new RelatedPostsViewModel
			{
				Posts = relatedPosts
			});
		}
	}

	public class RelatedPostsViewModel
	{
		public IReadOnlyList<RelatedPostViewModel> Posts { get; set; } = Array.Empty<RelatedPostViewModel>();
	}

	public class RelatedPostViewModel
	{
		public string Title { get; set; }
		public string Url { get; set; }
		public DateTime CreatedUtc { get; set; }
	}
}
