using DasBlog.Services.ActivityPub;

namespace DasBlog.Managers.Interfaces
{
	public interface IActivityPubManager
	{
		WebFinger WebFinger(string resource);
	}
}
