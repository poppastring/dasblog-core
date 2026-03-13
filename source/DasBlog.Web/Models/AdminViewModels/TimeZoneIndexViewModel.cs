using System.Collections.Generic;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class TimeZoneIndexViewModel
	{
		public string Name { get; set; }

		public decimal Id { get; set; }

		public List<TimeZoneIndexViewModel> Init()
		{
			return new List<TimeZoneIndexViewModel>() {
				new TimeZoneIndexViewModel { Id = -12m, Name = "UTC-12:00 Baker Island" },
				new TimeZoneIndexViewModel { Id = -11m, Name = "UTC-11:00 Pacific/Midway, Samoa" },
				new TimeZoneIndexViewModel { Id = -10m, Name = "UTC-10:00 Hawaii" },
				new TimeZoneIndexViewModel { Id = -9.5m, Name = "UTC-09:30 Marquesas Islands" },
				new TimeZoneIndexViewModel { Id = -9m, Name = "UTC-09:00 Alaska" },
				new TimeZoneIndexViewModel { Id = -8m, Name = "UTC-08:00 Pacific Time (US/Canada)" },
				new TimeZoneIndexViewModel { Id = -7m, Name = "UTC-07:00 Mountain Time (US/Canada)" },
				new TimeZoneIndexViewModel { Id = -6m, Name = "UTC-06:00 Central Time (US/Canada)" },
				new TimeZoneIndexViewModel { Id = -5m, Name = "UTC-05:00 Eastern Time (US/Canada)" },
				new TimeZoneIndexViewModel { Id = -4m, Name = "UTC-04:00 Atlantic Time (Canada)" },
				new TimeZoneIndexViewModel { Id = -3.5m, Name = "UTC-03:30 Newfoundland" },
				new TimeZoneIndexViewModel { Id = -3m, Name = "UTC-03:00 Brazil East, Argentina" },
				new TimeZoneIndexViewModel { Id = -2m, Name = "UTC-02:00 Mid-Atlantic" },
				new TimeZoneIndexViewModel { Id = -1m, Name = "UTC-01:00 Cape Verde, Azores" },
				new TimeZoneIndexViewModel { Id = 0m, Name = "UTC+00:00 GMT, London, Reykjavik" },
				new TimeZoneIndexViewModel { Id = 1m, Name = "UTC+01:00 Central Europe, West Africa" },
				new TimeZoneIndexViewModel { Id = 2m, Name = "UTC+02:00 Eastern Europe, Cairo" },
				new TimeZoneIndexViewModel { Id = 3m, Name = "UTC+03:00 Moscow, Nairobi, Baghdad" },
				new TimeZoneIndexViewModel { Id = 3.5m, Name = "UTC+03:30 Tehran" },
				new TimeZoneIndexViewModel { Id = 4m, Name = "UTC+04:00 Dubai, Muscat" },
				new TimeZoneIndexViewModel { Id = 4.5m, Name = "UTC+04:30 Kabul" },
				new TimeZoneIndexViewModel { Id = 5m, Name = "UTC+05:00 Karachi, Tashkent" },
				new TimeZoneIndexViewModel { Id = 5.5m, Name = "UTC+05:30 India, Sri Lanka" },
				new TimeZoneIndexViewModel { Id = 5.75m, Name = "UTC+05:45 Kathmandu" },
				new TimeZoneIndexViewModel { Id = 6m, Name = "UTC+06:00 Dhaka, Almaty" },
				new TimeZoneIndexViewModel { Id = 6.5m, Name = "UTC+06:30 Yangon" },
				new TimeZoneIndexViewModel { Id = 7m, Name = "UTC+07:00 Bangkok, Jakarta" },
				new TimeZoneIndexViewModel { Id = 8m, Name = "UTC+08:00 Hong Kong, Singapore, Perth" },
				new TimeZoneIndexViewModel { Id = 8.75m, Name = "UTC+08:45 Eucla (Australia)" },
				new TimeZoneIndexViewModel { Id = 9m, Name = "UTC+09:00 Tokyo, Seoul" },
				new TimeZoneIndexViewModel { Id = 9.5m, Name = "UTC+09:30 Adelaide, Darwin (Australia)" },
				new TimeZoneIndexViewModel { Id = 10m, Name = "UTC+10:00 Sydney, Brisbane (Australia)" },
				new TimeZoneIndexViewModel { Id = 10.5m, Name = "UTC+10:30 Lord Howe Island" },
				new TimeZoneIndexViewModel { Id = 11m, Name = "UTC+11:00 Solomon Islands, Noumea" },
				new TimeZoneIndexViewModel { Id = 12m, Name = "UTC+12:00 Auckland, Fiji" },
				new TimeZoneIndexViewModel { Id = 12.75m, Name = "UTC+12:45 Chatham Islands" },
				new TimeZoneIndexViewModel { Id = 13m, Name = "UTC+13:00 Samoa, Tonga" },
				new TimeZoneIndexViewModel { Id = 14m, Name = "UTC+14:00 Line Islands" },
			};
		}
	}
}
