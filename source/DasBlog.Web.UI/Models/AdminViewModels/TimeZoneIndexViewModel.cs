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
				new TimeZoneIndexViewModel { Id = -12, Name = "(-12)" },
				new TimeZoneIndexViewModel { Id = -11, Name = "Pacific/Midway, Pacific/Samoa (-11)" },
				new TimeZoneIndexViewModel { Id = -10, Name = "(-10)" },
				new TimeZoneIndexViewModel { Id = -9, Name = "US/Alaska (-9)" },
				new TimeZoneIndexViewModel { Id = -8, Name = "Canada/Pacific (-8)" },
				new TimeZoneIndexViewModel { Id = -7, Name = "Canada/Mountain (-7)" },
				new TimeZoneIndexViewModel { Id = -6, Name = "Canada/Central (-6)" },
				new TimeZoneIndexViewModel { Id = -5, Name = "US/Eastern (-5)" },
				new TimeZoneIndexViewModel { Id = -4, Name = "America/Anguilla (-4)" },
				new TimeZoneIndexViewModel { Id = -3, Name = "Brazil/East (-3)" },
				new TimeZoneIndexViewModel { Id = -2, Name = "Brazil/DeNoronha (-2)" },
				new TimeZoneIndexViewModel { Id = -1, Name = "(-1)" },
				new TimeZoneIndexViewModel { Id = -0, Name = "UTC (+0)" },
				new TimeZoneIndexViewModel { Id = 1, Name = "Poland (+1)" },
				new TimeZoneIndexViewModel { Id = 2, Name = "Africa/Mbabane (+2)" },
				new TimeZoneIndexViewModel { Id = 3, Name = "Asia/Bahrain (+3)" },
				new TimeZoneIndexViewModel { Id = 4, Name = "Asia/Muscat (+4)" },
				new TimeZoneIndexViewModel { Id = 5, Name = "Asia/Katmandu (+5)" },
				new TimeZoneIndexViewModel { Id = 6, Name = "Asia/Dacca (+6)" },
				new TimeZoneIndexViewModel { Id = 7, Name = "Asia/Saigon (+7)" },
				new TimeZoneIndexViewModel { Id = 8, Name = "Hongkong (+8)" },
				new TimeZoneIndexViewModel { Id = 9, Name = "Japan (+9)" },
				new TimeZoneIndexViewModel { Id = 10, Name = "Australia/Queensland (+10)" },
				new TimeZoneIndexViewModel { Id = 11, Name = "Pacific/Ponape (+11)" },
				new TimeZoneIndexViewModel { Id = 12, Name = "Antarctica/McMurdo (+12)" },

			};
		}
	}
}
