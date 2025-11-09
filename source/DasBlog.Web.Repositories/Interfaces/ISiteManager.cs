using DasBlog.Core.Services.GoogleSiteMap;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Managers.Interfaces
{
    public interface ISiteManager
    {
        UrlSet GetGoogleSiteMap();
    }
}
