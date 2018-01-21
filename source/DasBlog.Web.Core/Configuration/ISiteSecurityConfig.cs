using DasBlog.Web.Core.Security;
using System.Collections.Generic;

namespace DasBlog.Web.Core.Configuration
{
    public interface ISiteSecurityConfig
    {
        List<User> Users { get; set; }
    }
}
