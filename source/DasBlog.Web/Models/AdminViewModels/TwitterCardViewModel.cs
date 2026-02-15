using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class TwitterCardViewModel
	{
		public string Name { get; set; }

		public string Id { get; set; }

		public List<TwitterCardViewModel> Init()
		{
			return new List<TwitterCardViewModel>() {
				new TwitterCardViewModel { Id = "summary_large_image", Name = "Summary card with a large image" },
				new TwitterCardViewModel { Id = "summary", Name = "Summary card" }
			};
		}
	}
}
