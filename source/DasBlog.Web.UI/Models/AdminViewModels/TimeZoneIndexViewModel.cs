using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class TimeZoneIndexViewModel
	{
		public string Name { get; set; }

		public int Id { get; set; }

		public List<TimeZoneIndexViewModel> Init()
		{
			return new List<TimeZoneIndexViewModel>() {
				new TimeZoneIndexViewModel { Id = 0, Name = "" },
				new TimeZoneIndexViewModel { Id = 1, Name = "" },
			};
		}
	}
}
