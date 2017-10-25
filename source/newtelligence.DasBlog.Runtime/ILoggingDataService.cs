using System;

namespace newtelligence.DasBlog.Runtime
{
	public interface ILoggingDataService
	{
		LogDataItemCollection GetReferralsForDay(DateTime date);

		void AddReferral(LogDataItem logItem);

		LogDataItemCollection GetClickThroughsForDay(DateTime date);

		void AddClickThrough(LogDataItem logItem);

		LogDataItemCollection GetAggregatorBugHitsForDay(DateTime date);

		void AddAggregatorBugHit(LogDataItem logItem);

		LogDataItemCollection GetCrosspostReferrersForDay(DateTime date);

		void AddCrosspostReferrer(LogDataItem logItem);

		EventDataItemCollection GetEventsForDay(DateTime date);

		void AddEvent(EventDataItem eventData);
	}
}