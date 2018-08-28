using System;
using NodaTime;

namespace newtelligence.DasBlog.Runtime
{
    public class EntryCollectionFilter : CollectionFilter<EntryCollection, Entry>
    {

        /// <summary>
        /// Used to generate Predicate&lt;Entry&gt; delegates for default filters.
        /// </summary>
        public static class DefaultFilters
        {

            private class FilterContainer
            {
                public string CategoryName;
                public string UserName;
                public string AcceptLanguages;
                public string EntryId;
                public DateTime StartDateTime;
                public DateTime EndDateTime;
                public DateTime Month;
                public DateTimeZone TimeZone;
                public EntryCollection EntryCollection;

                public bool IsInCategory(Entry entry)
                {
                    return Entry.IsInCategory(entry, CategoryName);
                }

                public bool IsFromUser(Entry entry)
                {
                    if (entry.Author == null || UserName == null) return false;
                    return (entry.Author.ToUpper() == UserName.ToUpper());
                }

                public bool IsInAcceptedLanguagesOrMultiLingual(Entry entry)
                {
                    return (entry.Language == null) || (entry.Language == string.Empty)
                        || Entry.IsInAcceptedLanguages(entry, AcceptLanguages);
                }

                public bool OccursBefore(Entry entry)
                {
                    return Entry.OccursBefore(entry, TimeZone, EndDateTime);
                }

                public bool OccursBetween(Entry entry)
                {
                    return Entry.OccursBetween(entry, TimeZone, StartDateTime, EndDateTime);
                }

                public bool OccursInMonth(Entry entry)
                {
                    return Entry.OccursInMonth(entry, TimeZone, Month);
                }

                public bool IsInEntryCollection(Entry entry)
                {
                    return EntryCollection.ContainsKey(entry.EntryId);
                }

                public bool HasEntryId(Entry entry)
                {
                    return String.Compare(entry.EntryId, EntryId, StringComparison.InvariantCultureIgnoreCase) == 0;
                }
            }

            public static Predicate<Entry> IsPublic()
            {
                return new Predicate<Entry>(Entry.IsPublicEntry);
            }

            public static Predicate<Entry> IsInCategory(string categoryName)
            {
                FilterContainer container = new FilterContainer();

                container.CategoryName = categoryName;

                return new Predicate<Entry>(container.IsInCategory);
            }

            public static Predicate<Entry> IsFromUser(string userName)
            {
                FilterContainer container = new FilterContainer();
                container.UserName = userName;

                return new Predicate<Entry>(container.IsFromUser);
            }

            public static Predicate<Entry> IsInAcceptedLanguagesOrMultiLingual(string acceptLanguages)
            {
                FilterContainer container = new FilterContainer();

                container.AcceptLanguages = acceptLanguages;

                return new Predicate<Entry>(container.IsInAcceptedLanguagesOrMultiLingual);
            }


            public static Predicate<Entry> OccursBefore(DateTimeZone timeZone, DateTime dateTime)
            {
                FilterContainer container = new FilterContainer();

                container.TimeZone = timeZone;
                container.EndDateTime = dateTime;

                return new Predicate<Entry>(container.OccursBefore);
            }

            public static Predicate<Entry> OccursBetween(DateTimeZone timeZone, DateTime startDateTime, DateTime endDateTime)
            {
                FilterContainer container = new FilterContainer();

                container.TimeZone = timeZone;
                container.StartDateTime = startDateTime;
                container.EndDateTime = endDateTime;

                return new Predicate<Entry>(container.OccursBetween);
            }

            public static Predicate<Entry> OccursInMonth(DateTimeZone timeZone, DateTime month)
            {
                FilterContainer container = new FilterContainer();

                container.TimeZone = timeZone;
                container.Month = month;

                return new Predicate<Entry>(container.OccursInMonth);
            }

            public static Predicate<Entry> IsInEntryIdCacheEntryCollection(EntryCollection collection)
            {
                FilterContainer container = new FilterContainer();

                container.EntryCollection = collection;

                return new Predicate<Entry>(container.IsInEntryCollection);
            }

            public static Predicate<Entry> HasEntryId(string entryId)
            {
                FilterContainer container = new FilterContainer();

                container.EntryId = entryId;

                return new Predicate<Entry>(container.HasEntryId);
            }
        }
    }
}
