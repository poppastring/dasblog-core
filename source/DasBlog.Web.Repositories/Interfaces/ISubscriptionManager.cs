using DasBlog.Core.Services.Rsd;
using DasBlog.Core.Services.Rss20;

namespace DasBlog.Managers.Interfaces
{
    public interface ISubscriptionManager
    {
        RssRoot GetRss();
        RssRoot GetRssCategory(string categoryName);
        RssRoot GetAtom();
        RssRoot GetAtomCategory(string categoryName);
        RsdRoot GetRsd();
    }
}
