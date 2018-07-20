using System;
using System.Collections.Generic;
using System.Text;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Core.Extensions;
using Microsoft.Extensions.Logging;

namespace DasBlog.Core.Services
{
	/// <summary>
	/// serves up the infor from the log that we have deemed to be dasBlog events
	/// i.e. log entries from dasBlog code rather than Microsoft etc.
	/// </summary>
	public class ActivityService : IActivityService
	{
		private IActivityRepoFactory repoFactory;
		private IEventLineParser parser;
		private ILogger<ActivityService> logger;

		public ActivityService(IActivityRepoFactory repoFactory, IEventLineParser parser
		  ,ILogger<ActivityService> logger)
		{
			this.repoFactory = repoFactory;
			this.parser = parser;
			this.logger = logger;
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

			try
			{
				// process all the lines in the log for the given date
				// ignore info not related to DasBlog events i.e. Microsoft logging
				// and aggregate stack traces for dasBlog events with the event line
				using (var repo = repoFactory.GetRepo())
				{
					foreach (var line in repo.GetEventLines(date))
					{
						char[] chars = line.ToCharArray();
						if (chars.Length > 0 && !Char.IsDigit(chars[0])) goto stack_trace;
							// any line not starting with a date is treated as a stack trace frome
						(bool success, EventDataDisplayItem eddi) = parser.Parse(line);
						if (success) goto event_line;
						goto non_event_line;
								// any line that could not be parsed is assumed not to be a dasblog event
								// and is ignored
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
								stackTrace.Append("<br>");
								stackTrace.Append(line);
							}
							continue;
					}
				}

				if (isPreviousEvent)
				{
					UpdatePreviousEvent();
				}
			}
			catch (Exception e)
			{
				logger.LogError(new EventDataItem(EventCodes.Error, $"Failed to process the log file for {date.ToShortDateString()}", null), e);
				throw;
			}
			return events;
		}
	}
}
