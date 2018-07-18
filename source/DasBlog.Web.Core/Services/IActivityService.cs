using System;
using System.Collections.Generic;

namespace DasBlog.Core.Services
{
	public interface IActivityService
	{
		List<EventDataDisplayItem> GetEventsForDay(DateTime date);
	}
}
