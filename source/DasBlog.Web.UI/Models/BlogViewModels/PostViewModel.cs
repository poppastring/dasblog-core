using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.WindowsAzure.Storage.Blob.Protocol;
using System.Globalization;
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

		public IEnumerable<SelectListItem> Languages
		{
			get
			{
				return GetLanguages();
			}
		}

		private IEnumerable<SelectListItem> GetLanguages()
		{
			CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

			// setup temp store for listitem items, for sorting
			List<SelectListItem> cultureList = new List<SelectListItem>(cultures.Length);

			foreach (CultureInfo ci in cultures)
			{
				string langName = (ci.NativeName != ci.EnglishName) ? ci.NativeName + " / " + ci.EnglishName : ci.NativeName;

				if (langName.Length > 55)
				{
					langName = langName.Substring(0, 55) + "...";
				}

				cultureList.Add(new SelectListItem{ Value = ci.Name, Text = langName});
			}

			// setup the sort culture
			//string rssCulture = requestPage.SiteConfig.RssLanguage;

			CultureInfo sortCulture;

			try
			{
//				sortCulture = (rssCulture != null && rssCulture.Length > 0 ? new CultureInfo(rssCulture) : CultureInfo.CurrentCulture);
				sortCulture = CultureInfo.CurrentCulture;
			}
			catch (ArgumentException)
			{
				// default to the culture of the server
				sortCulture = CultureInfo.CurrentCulture;
			}

			// sort the list
			cultureList.Sort(delegate(SelectListItem x, SelectListItem y)
			{
				// actual comparison
				return String.Compare(x.Text, y.Text, true, sortCulture);
			});
			// add to the languages listbox

			SelectListItem[] cultureListItems = cultureList.ToArray();


			return cultureListItems;
/*
			listLanguages.Items.AddRange(cultureListItems);

			listLanguages.SelectedValue = "";
*/
		}
	}
}
