using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface ICategoryRepository
    {
        EntryCollection GetEntries();

        EntryCollection GetEntries(string category, string acceptLanguages);
    }
}
