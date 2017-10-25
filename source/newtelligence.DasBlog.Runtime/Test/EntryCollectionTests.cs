using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace newtelligence.DasBlog.Runtime.Test
{
    [TestFixture]
    public class EntryCollectionTests
    {

        [Test(Description="This test shows the CollectionFilter class does take chained delegates into account where, List<T>.FindAll does not.")]
        public void FilterTest()
        {

            Entry entry1 = CreateEntry("public 2007", true, new DateTime(2007, 2, 2));
            Entry entry2 = CreateEntry("public 2006", true, new DateTime(2006, 2, 2));
            Entry entry3 = CreateEntry("not public 2006", false, new DateTime(2006, 2, 2));

            EntryCollection collection = new EntryCollection(new Entry[] { entry1, entry2, entry3 });

            Predicate<Entry> filter = null;

            Predicate<Entry> isPublic = delegate(Entry e)
            {
                return e.IsPublic;
            };

            filter += isPublic;

            Predicate<Entry> is2006Published = delegate(Entry e)
            {
                return e.CreatedLocalTime.Year == 2006;
            };

            filter += is2006Published;

            EntryCollection classic = EntryCollectionFilter.FindAll(collection, filter);
            List<Entry> generic = collection.FindAll(filter);

            Assert.AreNotEqual(classic.Count, generic.Count, "Number of items doesn't match.");
        }

        [Test()]
        public void MaxResultsTest()
        {
            Entry entry1 = CreateEntry("public 2007.1", true, new DateTime(2007, 2, 2));
            Entry entry2 = CreateEntry("public 2006.1", true, new DateTime(2006, 2, 2));
            Entry entry3 = CreateEntry("not public 2006.1", false, new DateTime(2006, 2, 2));
            Entry entry4 = CreateEntry("public 2007.2", true, new DateTime(2007, 2, 2));
            Entry entry5 = CreateEntry("public 2006.2", true, new DateTime(2006, 2, 2));
            Entry entry6 = CreateEntry("not public 2006.2", false, new DateTime(2006, 2, 2));

            EntryCollection collection = new EntryCollection(new Entry[] { entry1, entry2, entry3, entry4, entry5, entry6 });

            Predicate<Entry> filter = null;

            Predicate<Entry> isPublic = delegate(Entry e)
            {
                return e.IsPublic;
            };

            filter += isPublic;

            Predicate<Entry> is2006Published = delegate(Entry e)
            {
                return e.CreatedLocalTime.Year == 2006;
            };

            filter += is2006Published;

            // 2 items match the criteria (2,5), we only want 1 returned
            EntryCollection filtered = EntryCollectionFilter.FindAll(collection, filter,1);

            Assert.AreEqual(1, filtered.Count, "Number of results doesn't match.");

            // no predicate so all results should be returend but we only want 4
            EntryCollection maxResults = EntryCollectionFilter.FindAll(collection, null, 4);

            Assert.AreEqual(4, maxResults.Count, "Number of results doesn't match.");

        }


        private static Entry CreateEntry(string title, bool isPublic, DateTime publishData)
        {

            Entry entry = new Entry();
            entry.Initialize();
            entry.Title = title;
            entry.IsPublic = isPublic;
            entry.CreatedLocalTime = publishData;

            return entry;
        }

        
    }
}
