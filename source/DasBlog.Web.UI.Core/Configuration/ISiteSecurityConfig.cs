using DasBlog.Web.UI.Core.Security;
using System.Collections.Generic;

namespace DasBlog.Web.UI.Core.Configuration
{
    public interface ISiteSecurityConfig
    {
        List<User> Users { get; set; }
    }
}
