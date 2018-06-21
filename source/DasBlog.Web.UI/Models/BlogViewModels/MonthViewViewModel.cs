using System;
using System.Collections.Generic;
using AutoMapper;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class MonthViewViewModel
	{
		private MonthViewViewModel(DateTime date)
		{
			var startOfTheMonth = new DateTime(date.Year, date.Month, 1);
			var startDayOfWeek = startOfTheMonth.DayOfWeek;
			var startOfCalendar = startOfTheMonth.AddDays(DayOfWeek.Sunday - startDayOfWeek);

			for (var day = 0; day < 35; day++)
			{
				MonthEntries.Add(startOfCalendar.AddDays(day).Date, new List<PostViewModel>());
			}
		}

		public Dictionary<DateTime, ICollection<PostViewModel>> MonthEntries { get; } = new Dictionary<DateTime, ICollection<PostViewModel>>();

		public static MonthViewViewModel Create(DateTime date, EntryCollection entries, IMapper mapper)
		{
			var m = new MonthViewViewModel(date);
			foreach (var entry in entries)
			{
				var post = mapper.Map<PostViewModel>(entry);
				m.MonthEntries[entry.CreatedLocalTime.Date].Add(post);
			}

			return m;
		}
	}
}
