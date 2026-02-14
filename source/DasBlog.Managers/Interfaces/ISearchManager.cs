using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
    public interface ISearchManager
    {
        EntryCollection SearchEntries(string searchString, string acceptLanguageHeader);
    }
}
