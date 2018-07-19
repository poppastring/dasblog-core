using System;
using System.Collections.Generic;
using System.Text;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Core.Services
{
	public class ActivityService : IActivityService
	{
		private IActivityRepoFactory repoFactory;
		private IEventLineParser parser;
		public ActivityService(IActivityRepoFactory repoFactory, IEventLineParser parser)
		{
			this.repoFactory = repoFactory;
			this.parser = parser;
		}
		public List<EventDataDisplayItem> GetEventsForDay(DateTime date)
		{
			List<EventDataDisplayItem> events = new List<EventDataDisplayItem>();
			bool isPreviousEvent = false;
			StringBuilder stackTrace = new StringBuilder();
			// add in the stacktrace to the last event
			void UpdatePreviousEvent()
			{
				EventDataDisplayItem existingEddi = events[events.Count - 1];

				var completeEddi = new EventDataDisplayItem(
					existingEddi.EventCode
					, existingEddi.HtmlMessage + stackTrace
					, existingEddi.Date);
				events[events.Count -1] = completeEddi;
				stackTrace.Clear();
			}
			using (var repo = repoFactory.GetRepo())
			{
				foreach (var line in repo.GetEventLines(date))
				{
					char[] chars = line.ToCharArray();
					if (chars.Length > 0 && !Char.IsDigit(chars[0])) goto stack_trace;
					(bool success, EventDataDisplayItem eddi) = parser.Parse(line);
					if (success) goto event_line;
					goto non_event_line;
					event_line:
					if (isPreviousEvent)	// previous event still in progress
					{
						UpdatePreviousEvent();
					}
					events.Add(eddi);
					isPreviousEvent = true;
					continue;
					non_event_line:
					if (isPreviousEvent)	// previous event still in progress
					{
						UpdatePreviousEvent();
					}
					isPreviousEvent = false;
					continue;
					stack_trace:
					if (isPreviousEvent)
					{
						stackTrace.Append(line);
					}
					continue;
				}
			}

			if (isPreviousEvent)
			{
				UpdatePreviousEvent();
			}
			return events;
		}
	}
}
