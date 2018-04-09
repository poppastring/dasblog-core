using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Services.Rsd;
using newtelligence.DasBlog.Web.Services.Rss20;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface ISubscriptionRepository
    {
        RssRoot GetRss();
        RssRoot GetRssCategory(string categoryName);
        RssRoot GetAtom();
        RssRoot GetAtomCategory(string categoryName);
        RsdRoot GetRsd();
    }
}
