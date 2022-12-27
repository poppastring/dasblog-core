using System.Collections.Generic;
using DasBlog.Services.ActivityPub;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
	public interface IActivityPubManager
	{
		WebFinger WebFinger(string resource);

		User GetUser();

		UserPage GetUserPage(IList<Entry> page);
	}
}
