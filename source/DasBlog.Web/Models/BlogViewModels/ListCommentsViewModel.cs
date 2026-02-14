using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class ListCommentsViewModel
    {
		public IList<CommentViewModel> Comments { get; set; }

		public string PostId { get; set; }

		public DateTime PostDate { get; set; }

		public string CommentUrl { get; set; }

		public bool ShowComments { get; set; }

		public bool AllowComments { get; set; }

		public AddCommentViewModel CurrentComment {get; set;} = null;
	}
}
