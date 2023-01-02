using DasBlog.Services.Rss.Rsd;
using DasBlog.Services.Rss.Rss20;

namespace DasBlog.Managers.Interfaces
{
    public interface ISubscriptionManager
    {
        RssRoot GetRss();
        RssRoot GetRssCategory(string categoryName);
        RssRoot GetAtom();
        RssRoot GetAtomCategory(string categoryName);
        RsdRoot GetRsd();
		RssItem GetRssItem(string entryId);
	}
}
