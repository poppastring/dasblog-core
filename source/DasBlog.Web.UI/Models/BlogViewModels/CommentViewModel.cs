using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class CommentViewModel
    {
		public string Name { get; set; }
		public string UserImageUrl { get; set; }
		public string Text { get; set; }
		public DateTime Date { get; set; }
	}
}
