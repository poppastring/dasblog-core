using System;
using System.Collections.Generic;

namespace DasBlog.Services.ActivityLogs
{
	public interface IActivityService
	{
		List<EventDataDisplayItem> GetEventsForDay(DateTime date);
	}
}
