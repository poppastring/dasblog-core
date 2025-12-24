using DasBlog.Services.Rss.Atom;
using DasBlog.Services.Rss.Rsd;
using DasBlog.Services.Rss.Rss20;

namespace DasBlog.Managers.Interfaces
{
    public interface ISubscriptionManager
    {
        RssRoot GetRss();
        RssRoot GetRssCategory(string categoryName);
        AtomRoot GetAtom();
        AtomRoot GetAtomCategory(string categoryName);
        RsdRoot GetRsd();
    }
}
