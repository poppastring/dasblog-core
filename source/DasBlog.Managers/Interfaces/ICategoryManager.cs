using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface ICategoryManager
    {
        EntryCollection GetEntries();

        EntryCollection GetEntries(string category, string acceptLanguages);

		string GetCategoryTitle(string category);
	}
}
