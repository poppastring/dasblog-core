using DasBlog.Services.ActivityPub;
using DasBlog.Services.Rss.Rss20;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
	public interface IActivityPubManager
	{
		WebFinger GetWebFinger();

		Actor GetActor();

		Outbox GenerateOutbox(EntryCollection rss);
	}
}
