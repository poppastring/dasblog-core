using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface IArchiveManager
    {
        EntryCollection GetEntriesForDay(DateTime date, string acceptLanguages);

        EntryCollection GetEntriesForMonth(DateTime date, string acceptLanguages);

        EntryCollection GetEntriesForYear(DateTime date, string acceptLanguages);

        List<DateTime> GetDaysWithEntries();
    }
}
