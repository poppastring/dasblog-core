using System;
using System.Collections.Generic;
using System.Linq;
using DasBlog.Web.Models.BlogViewModels;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class BlogPostListViewModel
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public List<BlogPostListViewModel> Init(List<PostViewModel> posts)
		{
			var allposts = posts.Select(p => new BlogPostListViewModel { Name = p.Title, Id = p.EntryId }).ToList();

			allposts.Insert(0, new BlogPostListViewModel { Name = "--Disable Pinning--", Id = "" });

			return allposts;
		}
	}
}
