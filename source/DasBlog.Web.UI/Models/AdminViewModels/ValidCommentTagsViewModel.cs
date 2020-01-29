using System.Collections.Generic;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class ValidCommentTagsViewModel
	{
		public List<TagViewModel> Tag { get; set; }

		public List<TagViewModel> Init()
		{
			return new List<TagViewModel>() {
				new TagViewModel { Name = "a", Attributes = "href,title" },
				new TagViewModel { Name = "b", Attributes = "" },
				new TagViewModel { Name = "blockquote", Attributes = "" },
				new TagViewModel { Name = "em", Attributes = "" },
				new TagViewModel { Name = "i", Attributes = "" },
				new TagViewModel { Name = "pre", Attributes = "" },
				new TagViewModel { Name = "strike", Attributes = "" },
				new TagViewModel { Name = "strong", Attributes = "" },
				new TagViewModel { Name = "sub", Attributes = "" },
				new TagViewModel { Name = "super", Attributes = "" },
				new TagViewModel { Name = "u", Attributes = "" },
				new TagViewModel { Name = "ul", Attributes = "" },
				new TagViewModel { Name = "ol", Attributes = "" },
				new TagViewModel { Name = "li", Attributes = "" }
			};
		}
	}
}

