using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

		private IList<CategoryViewModel> _categories = new List<CategoryViewModel>();
		public IList<CategoryViewModel> Categories
		{
			get { return _categories;}
			set { _categories = value; }
		}

		private IList<CategoryViewModel> _allCategories = new List<CategoryViewModel>();
		public IList<CategoryViewModel> AllCategories
		{
			get { return _allCategories;}
			set { _allCategories = value; }
		}

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

		public IEnumerable<SelectListItem> _languages = new List<SelectListItem>();
		public IEnumerable<SelectListItem> Languages
		{
			get { return _languages; }
			set { _languages = value; }
		}
	}
}
