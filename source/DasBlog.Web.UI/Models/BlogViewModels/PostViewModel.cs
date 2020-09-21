using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class PostViewModel
	{
		[Required]
		[StringLength(60, MinimumLength = 1)]
		public string Title { get; set; }

		[DataType(DataType.MultilineText)]
		public string Content { get; set; }

		[DataType(DataType.MultilineText)]
		public string Description { get; set; }

		public string Author { get; set; }

		public string PermaLink { get; set; }

		public string EntryId { get; set; }

		// categories associated with this blog post
		public IList<CategoryViewModel> Categories { get; set; } = new List<CategoryViewModel>();

		// all categories currently available on this blog
		public IList<CategoryViewModel> AllCategories { get; set; }= new List<CategoryViewModel>();

		public string NewCategory { get; set; }

		[Display(Name = "Allow Comments")]
		public bool AllowComments { get; set; }

		[Display(Name = "Is Public")]
		public bool IsPublic { get; set; }

		public bool Syndicated { get; set; }

		[Display(Name = "Date Created")]
		public DateTime CreatedDateTime { get; set; }

		[Display(Name = "Date Modified")]
		public DateTime ModifiedDateTime { get; set; }

		public ListCommentsViewModel Comments { get; set; }
		
		public IFormFile Image { get; set; }
		public string Language { get; set; }

		public IEnumerable<SelectListItem> Languages { get; set; }= new List<SelectListItem>();

		public string ImageUrl { get; set; } = string.Empty;

		public string VideoUrl { get; set; } = string.Empty;

		public int Order { get; set; } = 0;


        public List<string> ErrorMessages { get; set; }
    
	}
}
