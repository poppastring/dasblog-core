using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class BloggerAPIViewModel
	{
		public string Name { get; set; }

		public List<BloggerAPIViewModel> Init()
		{
			return new List<BloggerAPIViewModel>() {
				new BloggerAPIViewModel { Name = "Moveable Type" },
				new BloggerAPIViewModel { Name = "MetaWeblog" },
				new BloggerAPIViewModel { Name = "Blogger" }
			};

		}
	}
}
