using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class CommentAdminViewModel : CommentViewModel
	{
		public string Title { get; set; }
		public string Email { get; set; }
		public string AuthorIPAddress { get; set; }
	}
}
