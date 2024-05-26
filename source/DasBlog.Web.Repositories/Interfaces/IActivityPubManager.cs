using System.Threading.Tasks;
using DasBlog.Services.ActivityPub;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Managers.Interfaces
{
	public interface IActivityPubManager
	{
		WebFinger GetWebFinger();

		Actor GetActor();

		Outbox GenerateOutbox(EntryCollection entries);

		public Task Follow(InboxMessage message);

		public Task Unfollow(InboxMessage message, string requestbody);

		public Task Like(InboxMessage message);

		public Task AddReply(InboxMessage message);
	}
}
