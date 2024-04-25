using DasBlog.Services.ActivityPub;

namespace DasBlog.Managers.Interfaces
{
	public interface IActivityPubManager
	{
		WebFinger GetWebFinger();

		Actor GetActor();

		Outbox GenerateOutbox();
	}
}
