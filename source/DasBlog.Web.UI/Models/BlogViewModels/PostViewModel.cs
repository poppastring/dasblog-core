using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

		private IList<CategoryViewModel> _createdCategories = new List<CategoryViewModel>();
		public IList<CategoryViewModel> CreatedCategories
		{
			get { return _createdCategories;}
			set { _createdCategories = value; }
		}

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
	}
}
