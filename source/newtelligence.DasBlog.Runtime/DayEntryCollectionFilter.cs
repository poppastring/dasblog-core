using System;
using NodaTime;

namespace newtelligence.DasBlog.Runtime
{
    public class DayEntryCollectionFilter : CollectionFilter<DayEntryCollection, DayEntry>
    {

        /// <summary>
        /// Used to generate DayEntryCriteriaHandler delegates based on the date that
        /// the DayEntry should occur before.
        /// </summary>
        public static class DefaultFilters
        {
            private class FilterContainer
            {
                public DateTime StartDateTime;
                public DateTime EndDateTime;
                public DateTime Month;
                public DateTime day;
                public DateTimeZone TimeZone;

                public bool OccursBefore(DayEntry dayEntry)
                {
                    return DayEntry.OccursBefore(dayEntry, EndDateTime);
                }
                public bool OccursBetween(DayEntry dayEntry)
                {
                    return DayEntry.OccursBetween(dayEntry, TimeZone, StartDateTime, EndDateTime);
                }

                public bool OccursInMonth(DayEntry dayEntry)
                {
                    return DayEntry.OccursInMonth(dayEntry, TimeZone, Month);
                }

                public bool OccursOn(DayEntry dayEntry)
                {
                    return dayEntry.DateUtc == day;
                }
            }

            public static Predicate<DayEntry> OccursBefore(DateTime occursBeforeDateTime)
            {
                FilterContainer container = new FilterContainer();

                container.EndDateTime = occursBeforeDateTime;

                return container.OccursBefore;
            }

            public static Predicate<DayEntry> OccursBetween(DateTimeZone timeZone, DateTime startDateTime, DateTime endDateTime)
            {
                FilterContainer container = new FilterContainer();

                container.TimeZone = timeZone;
                container.StartDateTime = startDateTime;
                container.EndDateTime = endDateTime;

                return container.OccursBetween;
            }

            public static Predicate<DayEntry> OccursInMonth(DateTimeZone timeZone, DateTime month)
            {
                FilterContainer container = new FilterContainer();

                container.TimeZone = timeZone;
                container.Month = month;

                return container.OccursInMonth;
            }

            public static Predicate<DayEntry> OccursOn(DateTime key)
            {
                FilterContainer container = new FilterContainer();
                container.day = key;

                return container.OccursOn;
            }
        }
    }
}
