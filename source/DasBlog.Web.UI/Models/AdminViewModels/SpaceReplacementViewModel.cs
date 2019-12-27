using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class SpaceReplacementViewModel
	{
		public string Name { get; set; }

		public List<SpaceReplacementViewModel> Init()
		{
			return new List<SpaceReplacementViewModel>() {
				new SpaceReplacementViewModel { Name = "+" },
				new SpaceReplacementViewModel { Name = "-" }
			};
		}
	}
}
