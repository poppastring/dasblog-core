using DasBlog.Core.Security;
using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Core.Configuration
{
	public interface ISiteSecurityConfig
	{
		List<User> Users { get; set; }
	}
}
